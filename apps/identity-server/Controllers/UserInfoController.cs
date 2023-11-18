using IdentityServer.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers;

public class UserinfoController : Controller
{
  private readonly UserManager<AppUser> _userManager;

  public UserinfoController(UserManager<AppUser> userManager)
    => _userManager = userManager;

  [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
  [HttpGet("~/connect/userinfo"), Produces("application/json")]
  public async Task<IActionResult> Userinfo()
  {
    var user = await _userManager.FindByIdAsync(User.GetClaim(Claims.Subject));
    if (user is null)
    {
      return Challenge(new AuthenticationProperties(new Dictionary<string, string?>
        {
          [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
          [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Could not find the user."
        }),
        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    // Collect the claims.
    // See http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
    var claims = new Dictionary<string, object>(StringComparer.Ordinal)
    {
      // The `sub` claim serves as an identifier of the user.
      [Claims.Subject] = await _userManager.GetUserIdAsync(user)
    };

    if (User.HasScope(Scopes.Email))
    {
      // The `email` and `email_verified` claims refer to the email address of the user.
      claims[Claims.Email] = await _userManager.GetEmailAsync(user);
      claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
    }

    return Ok(claims);
  }
}
