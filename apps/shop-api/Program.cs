using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Validation.SystemNetHttp;
using Quartz;
using ShopApi;
using ShopApi.Data;
using ShopApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddControllers()
  .ConfigureApiBehaviorOptions(options =>
  {
    // Do not infer the binding source for controller action parameters. This allows to extract the
    // parameters into their own class and have different binding sources for each property.
    options.SuppressInferBindingSourcesForParameters = true;
  });

builder.Services.Configure<RouteOptions>(options =>
{
  // Enforce lowercase route prefixes e.g. /cart instead of /Cart for the CartController.
  options.LowercaseUrls = true;
});

builder.Services.AddSingleton<AppDbContext>();

// Use Quartz.NET to perform scheduled tasks e.g. seeding the database.
// This requires the `Quartz.Extensions.DependencyInjection` package to be installed.
builder.Services.AddQuartz(options =>
{
  options.UseInMemoryStore();
});

// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
// This requires the `Quartz.Extensions.Hosting` package to be installed.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

// Register Quartz.NET worker.
builder.Services.AddHostedService<Worker>();

// Add the Swagger generator to the services collection.
// See https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
{
  // Configure bearer authentication in the Swagger UI.
  // See https://swagger.io/docs/specification/authentication/bearer-authentication/

  // Define the HTTP bearer security scheme.
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Name = HeaderNames.Authorization,
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    BearerFormat = "JWT"
  });

  // Apply the security globally.
  options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      },
      Array.Empty<string>()
    }
  });
});

builder.Services
  .AddOpenIddict()

  // Register the OpenIddict validation components.
  .AddValidation(options =>
  {
    // Set the URL of the identity server that issues the tokens.
    options.SetIssuer("https://localhost:7001");

    // Configure the validation handler to use introspection. It uses OpenID Connect discovery
    // (.well-known/openid-configuration) to retrieve the URL of the introspection endpoint.
    options.UseIntrospection()
      .SetClientId("shop-api")
      .SetClientSecret("E3AE9B04-A744-49A2-8460-C027F8FDF343");

    // Register the System.Net.Http integration.
    options.UseSystemNetHttp();

    // Register the ASP.NET Core host.
    options.UseAspNetCore();
  });

// For now, make OpenIddict's System.Net.Http integration ignore server certificate validation errors.
// See https://stackoverflow.com/a/65618900
builder.Services
  .AddHttpClient(typeof(OpenIddictValidationSystemNetHttpOptions).Assembly.GetName().Name!)
  .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
  {
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
  });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  // Enable the middleware for serving the Swagger generated JSON document and Swagger UI.
  // See https://aka.ms/aspnetcore/swashbuckle
  app.UseSwagger();
  app.UseSwaggerUI();
}

// Configure CORS.
// See https://learn.microsoft.com/en-us/aspnet/core/security/cors
var allowedHosts = builder.Configuration["AllowedCorsOrigins"]!.Split(";");
app.UseCors(policyBuilder => policyBuilder
  .AllowAnyHeader()
  .AllowAnyMethod()
  .WithOrigins(allowedHosts));

// Enforce HTTPS.
// See https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
