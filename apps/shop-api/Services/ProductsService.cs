using Dapper;
using ShopApi.Data;

namespace ShopApi.Services;

public interface IProductsService
{
  Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
}

public class ProductsService : IProductsService
{
  private readonly IAppDbContextFactory _appDbContextFactory;

  public ProductsService(IAppDbContextFactory appDbContextFactory)
    => _appDbContextFactory = appDbContextFactory;

  public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    const string sql = @"
      SELECT Id, Name, Price, AlcoholByVolume
      FROM Product";

    return await context.Connection.QueryAsync<Product>(new CommandDefinition(sql,
      transaction: context.Transaction, cancellationToken: cancellationToken));
  }
}
