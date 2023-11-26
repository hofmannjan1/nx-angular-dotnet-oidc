using Dapper;
using ShopApi.Data;

namespace ShopApi;

public class Worker : IHostedService
{
  private readonly IServiceProvider _serviceProvider;

  public Worker(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await using var scope = _serviceProvider.CreateAsyncScope();

    // Seed the database.
    await CreateDatabaseTablesAsync(scope.ServiceProvider);
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  /// <summary>
  /// Create the required database tables.
  /// </summary>
  private static async Task CreateDatabaseTablesAsync(IServiceProvider serviceProvider)
  {
    var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();

    using var dbConnection = appDbContext.CreateConnection();

    var sql = @"
      CREATE TABLE IF NOT EXISTS
      Cart (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        UserId TEXT,
        ProductId INTEGER,
        Quantity INTEGER,
        -- Add UNIQUE constraint to allow upserting with the `ON CONFLICT` clause.
        UNIQUE(UserId, ProductId)
      )";

    await dbConnection.ExecuteAsync(sql);
  }
}
