using Dapper;
using ShopApi.Data;
using ShopApi.Services;

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
    await CreateProductsAsync(scope.ServiceProvider);
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
      );
      CREATE TABLE IF NOT EXISTS
      Product (
        Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
        [Name] VARCHAR(255),
        Price REAL,
        AlcoholByVolume REAL,
        -- Add UNIQUE constraint to prevent duplicates with the `INSERT OR IGNORE` clause.
        UNIQUE([Name])
      );";

    await dbConnection.ExecuteAsync(sql);
  }

  private static async Task CreateProductsAsync(IServiceProvider serviceProvider)
  {
    var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();

    using var dbConnection = appDbContext.CreateConnection();

    var sql = @"
      INSERT OR IGNORE INTO Product([Name], Price, AlcoholByVolume)
      VALUES(@Name, @Price, @AlcoholByVolume);";

    await dbConnection.ExecuteAsync(sql, new Product[]
    {
      new()
      {
        Name = "Lagavulin 16 Year Old Single Malt Scotch Whisky",
        Price = (decimal) 69.9,
        AlcoholByVolume = 43
      },
      new()
      {
        Name = "Ardbeg 10 Year Old Single Malt Scotch Whisky",
        Price = (decimal) 39.9,
        AlcoholByVolume = 46
      },
      new()
      {
        Name = "Knob Creek 9 Year Old Small Batch Kentucky Straight Bourbon Whiskey",
        Price = (decimal) 29.9,
        AlcoholByVolume = 50
      },
      new()
      {
        Name = "Wild Turkey Rare Breed Barrel Proof Kentucky Straight Bourbon Whiskey",
        Price = (decimal) 39.9,
        AlcoholByVolume = (decimal) 58.4
      },
      new()
      {
        Name = "Bushmills 12 Year Old Single Malt Irish Whiskey",
        Price = (decimal) 49.9,
        AlcoholByVolume = 40
      },
      new()
      {
        Name = "Green Spot Single Pot Still Irish Whiskey",
        Price = (decimal) 49.9,
        AlcoholByVolume = 40
      },
      new()
      {
        Name = "Montelobos Mezcal Espadín",
        Price = (decimal) 39.9,
        AlcoholByVolume = (decimal) 43.2
      },
      new()
      {
        Name = "Marca Negra Mezcal Espadín",
        Price = (decimal) 59.9,
        AlcoholByVolume = (decimal) 49.2
      },
      new()
      {
        Name = "Hampden Estate 8 Year Old Pure Single Jamaican Rum",
        Price = (decimal) 59.9,
        AlcoholByVolume = 46
      },
      new()
      {
        Name = "Appleton Estate 12 Year Old Blended Jamaican Rum",
        Price = (decimal) 34.9,
        AlcoholByVolume = 43
      }
    });
  }
}
