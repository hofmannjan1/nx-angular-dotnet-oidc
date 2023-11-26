using Dapper;
using ShopApi.Data;

namespace ShopApi.Services;

public interface ICartService
{
  Task<IEnumerable<CartPosition>> GetCartPositionsAsync(string userId,
    CancellationToken cancellationToken);

  Task<int> CreateCartPositionAsync(string userId, int productId, int? quantity,
    CancellationToken cancellationToken);
}

public class CartService : ICartService
{
  private readonly AppDbContext _appDbContext;

  public CartService(AppDbContext appDbContext)
  {
    _appDbContext = appDbContext;
  }

  public async Task<IEnumerable<CartPosition>> GetCartPositionsAsync(string userId,
    CancellationToken cancellationToken)
  {
    using var dbConnection = _appDbContext.CreateConnection();

    var sql = @"
      SELECT Id, UserId, ProductId, Quantity
      FROM Cart
      WHERE UserId = @UserId";

    return await dbConnection.QueryAsync<CartPosition>(new CommandDefinition(sql, new
    {
      UserId = userId
    }, cancellationToken: cancellationToken));
  }

  public async Task<int> CreateCartPositionAsync(string userId, int productId, int? quantity,
    CancellationToken cancellationToken)
  {
    using var dbConnection = _appDbContext.CreateConnection();

    var sql = @"
      INSERT INTO Cart(UserId, ProductId, Quantity)
      VALUES (@UserId, @ProductId, @Quantity);
      SELECT last_insert_rowid()";

    return await dbConnection.QuerySingleAsync<int>(new CommandDefinition(sql, new
    {
      UserId = userId,
      ProductId = productId,
      Quantity = quantity ?? 1
    }, cancellationToken: cancellationToken));
  }
}
