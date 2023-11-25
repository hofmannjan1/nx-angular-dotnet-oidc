using IdentityServer.Data;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer;

public class Worker : IHostedService
{
  private readonly IServiceProvider _serviceProvider;

  public Worker(IServiceProvider serviceProvider)
    => _serviceProvider = serviceProvider;

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await using var scope = _serviceProvider.CreateAsyncScope();

    // Ensure that the database for the context exists.
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync(cancellationToken);

    // Seed the database.
    await RegisterApplicationsAsync(scope.ServiceProvider);
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  private static async Task RegisterApplicationsAsync(IServiceProvider serviceProvider)
  {
    var applicationManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    if (await applicationManager.FindByClientIdAsync("shop") is null)
    {
      await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
      {
        // The unique client id for the Angular Shop application.
        ClientId = "shop",
        // Explicit consent by the user is required for the application to access the user data.
        ConsentType = ConsentTypes.Explicit,
        // The client type specifies if the client is able to share confidential credentials with the identity server.
        // See https://datatracker.ietf.org/doc/html/rfc6749#section-2.1
        Type = ClientTypes.Public,
        DisplayName = "Angular Shop",
        // The redirect uri passed by the application must match one of these.
        RedirectUris =
        {
          new Uri("http://localhost:5201"),
          new Uri("http://127.0.0.1:5201")
        },
        PostLogoutRedirectUris =
        {
          new Uri("http://localhost:5201"),
          new Uri("http://127.0.0.1:5201")
        },
        Permissions =
        {
          // The application is permitted to use the endpoints.
          Permissions.Endpoints.Authorization,
          Permissions.Endpoints.Token,
          // The application is permitted to use the authorization code flow with refresh tokens.
          Permissions.GrantTypes.AuthorizationCode,
          Permissions.GrantTypes.RefreshToken,
          Permissions.ResponseTypes.Code,
          // The application is permitted to use the scopes.
          Permissions.Scopes.Email,
          Permissions.Scopes.Profile
        },
        Requirements =
        {
          // The application is required to use PKCE.
          // See https://documentation.openiddict.com/configuration/proof-key-for-code-exchange.html
          Requirements.Features.ProofKeyForCodeExchange
        }
      });
    }

    if (await applicationManager.FindByClientIdAsync("shop-api") is null)
    {
      await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
      {
        ClientId = "shop-api",
        ClientSecret = "E3AE9B04-A744-49A2-8460-C027F8FDF343",
        Permissions =
        {
          // The resource server is permitted to use the introspection endpoint to validate tokens.
          Permissions.Endpoints.Introspection
        }
      });
    }
  }
}
