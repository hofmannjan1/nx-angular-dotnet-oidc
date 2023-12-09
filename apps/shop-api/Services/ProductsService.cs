using Dapper;
using ShopApi.Data;

namespace ShopApi.Services;

public interface IProductsService
{
  Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
}

public class ProductsService : IProductsService
{
  private readonly AppDbContext _appDbContext;

  public ProductsService(AppDbContext appDbContext)
    => _appDbContext = appDbContext;

  public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
  {
    using var connection = _appDbContext.CreateConnection();

    const string sql = @"
      SELECT Id, Name, Price, AlcoholByVolume
      FROM Product";

    return await connection.QueryAsync<Product>(new CommandDefinition(sql,
      cancellationToken: cancellationToken));
  }
}
