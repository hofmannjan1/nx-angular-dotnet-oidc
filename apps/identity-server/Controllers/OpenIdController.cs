using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers;

public class OpenIdController : Controller
{
  /// <summary>
  /// Determines whether the specified claim is included in the access token, the identity token or in both.
  /// </summary>
  protected static readonly Func<Claim, IEnumerable<string>> ClaimsDestinationSelector = claim => claim switch
  {
    { Type: Claims.Name } => claim.Subject!.HasScope(Permissions.Scopes.Profile)
      ? new [] { Destinations.AccessToken,  Destinations.IdentityToken }
      : new [] { Destinations.AccessToken },

    { Type: Claims.Email } => claim.Subject!.HasScope(Permissions.Scopes.Email)
      ? new [] { Destinations.AccessToken,  Destinations.IdentityToken }
      : new [] { Destinations.AccessToken },

    _ => new [] { Destinations.AccessToken }
  };
}
