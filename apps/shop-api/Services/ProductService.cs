using Dapper;
using ShopApi.Data;

namespace ShopApi.Services;

public interface IProductService
{
  Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
}

public class ProductService : IProductService
{
  private readonly AppDbContext _appDbContext;

  public ProductService(AppDbContext appDbContext)
    => _appDbContext = appDbContext;

  public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
  {
    using var dbConnection = _appDbContext.CreateConnection();

    const string sql = @"
      SELECT Id, Name, Price, AlcoholByVolume
      FROM Product";

    return await dbConnection.QueryAsync<Product>(new CommandDefinition(sql,
      cancellationToken: cancellationToken));
  }
}
