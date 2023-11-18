using IdentityServer.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace IdentityServer.Controllers;

public class LogoutController : Controller
{
  private readonly SignInManager<AppUser> _signInManager;

  public LogoutController(SignInManager<AppUser> signInManager)
    => _signInManager = signInManager;

  [HttpGet("~/connect/logout")]
  public IActionResult Logout() => View();

  [HttpPost("~/connect/logout"), ActionName("Logout")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> LogoutPost()
  {
    await _signInManager.SignOutAsync();

    // Redirect to the `post_logout_redirect_uri` specified by the client or, otherwise, to the `RedirectUri`.
    return SignOut(new AuthenticationProperties { RedirectUri = "/" }, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }
}
