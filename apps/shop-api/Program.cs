using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Validation.SystemNetHttp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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
