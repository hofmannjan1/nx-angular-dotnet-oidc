using System.Security.Claims;
using IdentityServer.Data;
using IdentityServer.ViewModels.Authorize;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers;

public class AuthorizeController : OpenIdController
{
  private readonly IOpenIddictApplicationManager _applicationManager;
  private readonly IOpenIddictAuthorizationManager _authorizationManager;
  private readonly IOpenIddictScopeManager _scopeManager;
  private readonly UserManager<AppUser> _userManager;

  public AuthorizeController(UserManager<AppUser> userManager,
    IOpenIddictApplicationManager applicationManager,
    IOpenIddictAuthorizationManager authorizationManager,
    IOpenIddictScopeManager scopeManager)
  {
    _userManager = userManager;
    _applicationManager = applicationManager;
    _authorizationManager = authorizationManager;
    _scopeManager = scopeManager;
  }

  /// <summary>
  /// Handle the authorization request from the client application as part of the authorization code flow.
  /// </summary>
  [HttpGet("~/connect/authorize"), HttpPost("~/connect/authorize")]
  [IgnoreAntiforgeryToken]
  public async Task<IActionResult> AuthorizeAsync()
  {
    // Retrieve the OpenID Connect request.
    var request = HttpContext.GetOpenIddictServerRequest()
      ?? throw new InvalidOperationException("Could not retrieve the request.");

    // Redirect to login if the client application explicitly requests the login prompt.
    if (request.HasPrompt(Prompts.Login))
      return await LoginChallengeResultAsync();

    // Retrieve the claims principal.
    var result = await HttpContext.AuthenticateAsync()
      ?? throw new InvalidOperationException("Could not retrieve the claims principal.");

    if (!result.Succeeded || request.MaxAge.HasValue && result.Properties.IssuedUtc.HasValue
      && DateTimeOffset.UtcNow - result.Properties.IssuedUtc.Value > TimeSpan.FromSeconds(request.MaxAge.Value))
    {
      // Return an error if the client requested promptless authentication.
      if (request.HasPrompt(Prompts.None))
        return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Could not log in the user."
          }),
          OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

      return await LoginChallengeResultAsync();
    }

    // Retrieve the profile of the logged in user.
    var user = await _userManager.GetUserAsync(result.Principal)
      ?? throw new InvalidOperationException("Could not retrieve the user details.");

    // Retrieve the application details from the database.
    var application = await _applicationManager.FindByClientIdAsync(request.ClientId)
      ?? throw new InvalidOperationException("Could not retrieve the client application details.");

    // Retrieve the permanent authorizations for the user, client and scopes.
    var authorizations = await _authorizationManager.FindAsync(
      await _userManager.GetUserIdAsync(user),
      await _applicationManager.GetIdAsync(application),
      Statuses.Valid,
      AuthorizationTypes.Permanent,
      request.GetScopes()).ToListAsync();

    // For now, require explicit user consent.
    if (!await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.Explicit))
      return Forbid(
        new AuthenticationProperties(new Dictionary<string, string?>
        {
          [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
          [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Explicit user consent is required."
        }),
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    // Return an authorization response without displaying the consent form.
    // This would also apply for implicit consent.
    if(authorizations.Any() && !request.HasPrompt(Prompts.Consent))
    {
      return await AuthorizationResultAsync();
    }

    // At this point, authorization failed. Return an error if the client requested promptless authentication.
    if (request.HasPrompt(Prompts.None))
      return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
        {
          [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
          [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Explicit user consent is required."
        }),
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    // Render the consent form.
    return View(new AuthorizeViewModel
    {
      ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
      Scope = request.Scope
    });
  }

  [Authorize]
  [HttpPost("~/connect/authorize"), FormValueRequired("submit.Accept")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> AcceptAsync()
  {
    // Retrieve the OpenID Connect request.
    var request = HttpContext.GetOpenIddictServerRequest()
      ?? throw new InvalidOperationException("Could not retrieve the request.");

    // Retrieve the application details from the database.
    var application = await _applicationManager.FindByClientIdAsync(request.ClientId)
      ?? throw new InvalidOperationException("Could not retrieve the client application details.");

    // For now, require explicit user consent.
    if (!await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.Explicit))
      return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
        {
          [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
          [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Explicit user consent is required."
        }),
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    return await AuthorizationResultAsync();
  }

  /// <summary>
  /// Trigger redirect to the client application if the authorization grant is denied by the user.
  /// </summary>
  [Authorize]
  [HttpPost("~/connect/authorize"), FormValueRequired("submit.Deny")]
  [ValidateAntiForgeryToken]
  public IActionResult Deny() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

  /// <summary>
  /// Enforce login.
  /// </summary>
  private async Task<ChallengeResult> LoginChallengeResultAsync()
  {
    // Retrieve the OpenID Connect request.
    var request = HttpContext.GetOpenIddictServerRequest()
      ?? throw new InvalidOperationException("Could not retrieve the request.");

    // Remove the login prompt flag to avoid endless redirects.
    var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));

    var parameters = Request.HasFormContentType
      ? Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
      : Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

    parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

    return Challenge(new AuthenticationProperties
    {
      RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
    });
  }

  /// <summary>
  /// Return authorization result.
  /// </summary>
  private async Task<Microsoft.AspNetCore.Mvc.SignInResult> AuthorizationResultAsync()
  {
    // Retrieve the OpenID Connect request.
    var request = HttpContext.GetOpenIddictServerRequest()
      ?? throw new InvalidOperationException("Could not retrieve the request.");

    // Retrieve the profile of the logged in user.
    var user = await _userManager.GetUserAsync(User)
      ?? throw new InvalidOperationException("Could not retrieve the user details.");

    // Retrieve the application details from the database.
    var application = await _applicationManager.FindByClientIdAsync(request.ClientId)
      ?? throw new InvalidOperationException("Could not retrieve the client application details.");

    // Create the claim-based identity for generating the token.
    var identity = new ClaimsIdentity(
      TokenValidationParameters.DefaultAuthenticationType,
      Claims.Name,
      Claims.Role);

    // Add the claims that will be persisted in the generated token.
    identity
      .SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
      .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
      .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user));

    // For now, the granted scopes match the requested scopes e.g. the user cannot uncheck specific scopes.
    identity.SetScopes(request.GetScopes());
    identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

    // Retrieve the permanent authorizations for the user and client application. Otherwise, create a permanent
    // authorization that will not require explicit consent for future authorization requests with the same scopes.
    var authorizations = await _authorizationManager.FindAsync(
      await _userManager.GetUserIdAsync(user),
      await _applicationManager.GetIdAsync(application),
      Statuses.Valid,
      AuthorizationTypes.Permanent,
      request.GetScopes()).ToListAsync();

    var authorization = authorizations.LastOrDefault();

    authorization ??= await _authorizationManager.CreateAsync(
      identity,
      await _userManager.GetUserIdAsync(user),
      await _applicationManager.GetIdAsync(application),
      AuthorizationTypes.Permanent,
      identity.GetScopes());

    identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

    // Set destinations (access token, identity token or both) for the claims.
    identity.SetDestinations(ClaimsDestinationSelector);

    // Return authorization result.
    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }
}

public sealed class FormValueRequiredAttribute : ActionMethodSelectorAttribute
{
  private readonly string _name;

  public FormValueRequiredAttribute(string name)
    => _name = name;

  public override bool IsValidForRequest(RouteContext context, ActionDescriptor action)
    => string.Equals(context.HttpContext.Request.Method, "POST", StringComparison.OrdinalIgnoreCase)
      && !string.IsNullOrEmpty(context.HttpContext.Request.ContentType)
      && context.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)
      && !string.IsNullOrEmpty(context.HttpContext.Request.Form[_name]);
}
