using System.Security.Claims;
using IdentityServer.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers;

public class TokenController : OpenIdController
{
  private readonly SignInManager<AppUser> _signInManager;
  private readonly UserManager<AppUser> _userManager;
  private readonly IOpenIddictScopeManager _scopeManager;

  public TokenController(SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager, IOpenIddictScopeManager scopeManager)
  {
    _signInManager = signInManager;
    _userManager = userManager;
    _scopeManager = scopeManager;
  }

  [HttpPost("~/connect/token"), Produces("application/json")]
  [IgnoreAntiforgeryToken]
  public async Task<IActionResult> TokenAsync()
  {
    // Retrieve the OpenID Connect request.
    var request = HttpContext.GetOpenIddictServerRequest()
      ?? throw new InvalidOperationException("Could not retrieve the request.");

    if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
      throw new InvalidOperationException("The grant type is not supported.");

    // Retrieve the claims principal stored in the authorization code/refresh token.
    var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
      ?? throw new InvalidOperationException("Could not retrieve the claims principal.");

    if (!result.Succeeded)
      throw new InvalidOperationException("Could not authenticate.");

    // Retrieve the user profile corresponding to the authorization code/refresh token.
    var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(Claims.Subject)!);
    if (user is null)
      return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
        {
          [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
          [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Could not find the user."
        }),
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    // Ensure the user is allowed to sign in.
    if (!await _signInManager.CanSignInAsync(user))
      return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
        {
          [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
          [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not allowed to sign in."
        }),
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    // Initialize claims identity.
    var identity = new ClaimsIdentity(
      result.Principal.Claims,
      TokenValidationParameters.DefaultAuthenticationType,
      Claims.Name,
      Claims.Role);

    // Override claims in the principal in case they changed since the authorization code/refresh token was issued.
    identity
      .SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
      .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
      .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user));

    // Grant all the requested scopes.
    identity.SetScopes(request.GetScopes());

    // Get the resource servers associated with the granted scopes and set them as audience.
    identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

    // Set destinations (access token, identity token or both) for the claims.
    identity.SetDestinations(ClaimsDestinationSelector);

    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }
}
