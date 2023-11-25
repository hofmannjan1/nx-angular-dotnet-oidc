var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add the Swagger generator to the services collection
// See https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  // Enable the middleware for serving the Swagger generated JSON document and Swagger UI
  // See https://aka.ms/aspnetcore/swashbuckle
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
