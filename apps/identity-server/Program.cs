using IdentityServer;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register the database context as a service.
builder.Services.AddDbContext<AppDbContext>(options =>
{
  // Store the database in a temporary folder.
  var tempPath = Path.Combine(Path.GetTempPath(), "nx-angular-dotnet-oidc");
  if (!File.Exists(tempPath)) {
    Directory.CreateDirectory(tempPath);
  }

  // Use Sqlite as database engine. Feel free to use e.g SqlServer instead.
  // This requires the `Microsoft.EntityFrameworkCore.Sqlite` package to be installed.
  options.UseSqlite($"Filename={Path.Combine(tempPath, "identity-server.sqlite3")}");

  // Register the default OpenIddict entity sets in the Entity Framework Core context.
  options.UseOpenIddict();
});

// Use the OpenIddict integration with Quartz.NET to perform scheduled tasks e.g. seeding the database.
// This requires the `Quartz.Extensions.DependencyInjection` package to be installed.
builder.Services.AddQuartz(options =>
{
  options.UseSimpleTypeLoader();
  options.UseInMemoryStore();
});

// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
// This requires the `Quartz.Extensions.Hosting` package to be installed.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

// Register Quartz.NET worker.
builder.Services.AddHostedService<Worker>();

// Register the Identity services.
builder.Services
  .AddIdentity<AppUser, IdentityRole>()
  // This requires the `Microsoft.AspNetCore.Identity.EntityFrameworkCore` package to be installed.
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders()
  // The default UI provides the Identity pages for managing user info, email, password and two-factor authentication.
  // This requires the `Microsoft.AspNetCore.Identity.UI` package to be installed.
  .AddDefaultUI();

builder.Services
  .AddOpenIddict()
  // Register the OpenIddict core components.
  .AddCore(options =>
  {
    // Configure OpenIddict to use the Entity Framework Core stores and default entities.
    options
      .UseEntityFrameworkCore()
      .UseDbContext<AppDbContext>();

    // Enable Quartz.NET integration.
    options.UseQuartz();
  })
  // Register the OpenIddict server components.
  // OpenIddict will automatically provide an endpoint /.well-known/openid-configuration
  .AddServer(options =>
  {
    // Register `profile` and `email` as supported scopes.
    options.RegisterScopes(Scopes.Profile, Scopes.Email);

    options
      // Enable the authorization endpoint for the authorization code flow.
      .SetAuthorizationEndpointUris("connect/authorize")
      // Enable the token endpoint.
      .SetTokenEndpointUris("connect/token")
      // Enable the userinfo endpoint.
      .SetUserinfoEndpointUris("connect/userinfo")
      // Enable the introspection endpoint.
      // See https://www.oauth.com/oauth2-servers/token-introspection-endpoint
      .SetIntrospectionEndpointUris("connect/introspection")
      // Enable the logout endpoint.
      .SetLogoutEndpointUris("connect/logout");

    // Enable authorization code flow with refresh tokens.
    options
      .AllowAuthorizationCodeFlow()
      .AllowRefreshTokenFlow();

    // Register the signing and encryption credentials.
    options
      .AddDevelopmentEncryptionCertificate()
      .AddDevelopmentSigningCertificate();

    // Register the .NET host.
    options
      .UseAspNetCore()
      // Allow the custom controller action to handle authorization requests.
      .EnableAuthorizationEndpointPassthrough()
      // Allow the custom controller action to handle token requests.
      .EnableTokenEndpointPassthrough()
      // Allow the custom controller action to handle userinfo requests.
      .EnableUserinfoEndpointPassthrough()
      // Allow the custom controller action to handle logout requests.
      .EnableLogoutEndpointPassthrough();
  })
  // Register the OpenIddict validation components.
  .AddValidation(options =>
  {
    // Import the configuration from the local OpenIddict server instance.
    options.UseLocalServer();

    // Register the .NET host.
    options.UseAspNetCore();
  });

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

// Use static files from wwwroot
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
  // For development, use static files from ../../node_modules/bootstrap
  // These files are copied to wwwroot on `build` or `publish`
  app.UseStaticFiles(new StaticFileOptions
  {
    FileProvider = new PhysicalFileProvider(
      Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "node_modules/bootstrap/dist/css")),
    RequestPath = "/bootstrap/dist/css"
  });
}

// Configure CORS.
// See https://learn.microsoft.com/en-us/aspnet/core/security/cors
var allowedHosts = builder.Configuration["AllowedCorsOrigins"]!.Split(";");
app.UseCors(policyBuilder => policyBuilder
  .AllowAnyHeader()
  .AllowAnyMethod()
  .WithOrigins(allowedHosts));

app.UseRouting();

// Enforce HTTPS.
// See https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Map / to {controller=Home}/{action=Index}
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
