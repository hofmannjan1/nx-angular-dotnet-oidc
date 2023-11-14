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

// Use the OpenIddict integration with Quartz.NET to perform scheduled tasks.
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

// Use static files from wwwroot
app.UseStaticFiles();
// Use static files from ../../node_modules/bootstrap
// For `build` and `publish` these files are copied to wwwroot
if (app.Environment.IsDevelopment())
{
  app.UseStaticFiles(new StaticFileOptions
  {
    FileProvider = new PhysicalFileProvider(
      Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "node_modules/bootstrap/dist/css")),
    RequestPath = "/bootstrap/dist/css"
  });
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Map / to {controller=Home}/{action=Index}
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
