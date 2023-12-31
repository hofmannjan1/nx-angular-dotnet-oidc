using Dapper;
using Microsoft.Data.SqlClient;
using ShopApi.Data;

namespace ShopApi;

public class Worker : IHostedService
{
  private readonly IServiceProvider _serviceProvider;
  private readonly IConfiguration _configuration;

  public Worker(IServiceProvider serviceProvider, IConfiguration configuration)
  {
    _serviceProvider = serviceProvider;
    _configuration = configuration;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    // Create directory if it does not exist.
    var connString = new SqlConnectionStringBuilder(_configuration.GetConnectionString("Sqlite"));
    var directory = Path.GetDirectoryName(connString["Data Source"] as string);

    if (directory is not null & !File.Exists(directory))
      Directory.CreateDirectory(directory!);
    
    await using var scope = _serviceProvider.CreateAsyncScope();

    // Seed the database.
    await CreateDatabaseTablesAsync(scope.ServiceProvider);
    await CreateProductsAsync(scope.ServiceProvider);
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  /// <summary>
  /// Create the required database tables.
  /// </summary>
  private static async Task CreateDatabaseTablesAsync(IServiceProvider serviceProvider)
  {
    var factory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

    await using var context = factory.CreateContext();

    const string sql = @"
      CREATE TABLE IF NOT EXISTS
      Cart (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        UserId TEXT,
        ProductId INTEGER,
        Quantity INTEGER,
        -- Add UNIQUE constraint to allow upserting with the `ON CONFLICT` clause.
        UNIQUE(UserId, ProductId)
      );
      CREATE TABLE IF NOT EXISTS
      Product (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        [Name] VARCHAR(255),
        Price REAL,
        AlcoholByVolume REAL,
        -- Add UNIQUE constraint to prevent duplicates with the `INSERT OR IGNORE` clause.
        UNIQUE([Name])
      );
      CREATE TABLE IF NOT EXISTS
      [Order] (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        UserId TEXT,
        [DateTime] TEXT
      );
      CREATE TABLE IF NOT EXISTS
      OrderPosition (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        OrderId INTEGER,
        ProductId INTEGER,
        Quantity INTEGER,
        Price REAL,
        -- Add UNIQUE constraint to allow upserting with the `ON CONFLICT` clause.
        UNIQUE(OrderId, ProductId)
      );";

    await context.Connection.ExecuteAsync(sql);
  }

  private static async Task CreateProductsAsync(IServiceProvider serviceProvider)
  {
    var factory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

    await using var context = factory.CreateContext();

    const string sql = @"
      INSERT OR IGNORE INTO Product([Name], Price, AlcoholByVolume)
      VALUES
        ('Lagavulin 16 Year Old Single Malt Scotch Whisky', 69.9, 43),
        ('Ardbeg 10 Year Old Single Malt Scotch Whisky', 39.9, 46),
        ('Knob Creek 9 Year Old Small Batch Kentucky Straight Bourbon Whiskey', 29.9, 50),
        ('Wild Turkey Rare Breed Barrel Proof Kentucky Straight Bourbon Whiskey', 39.9, 58.4),
        ('Bushmills 12 Year Old Single Malt Irish Whiskey', 49.9, 40),
        ('Green Spot Single Pot Still Irish Whiskey', 49.9, 40),
        ('Montelobos Mezcal Espadín', 39.9, 43.2),
        ('Marca Negra Mezcal Espadín', 59.9, 49.2),
        ('Hampden Estate 8 Year Old Pure Single Jamaican Rum', 59.9, 46),
        ('Appleton Estate 12 Year Old Blended Jamaican Rum', 34.9, 43);";

    await context.Connection.ExecuteAsync(sql);
  }
}
