/*
 * ABOUT THIS FILE
 *
 * This files encapsulates the database access to perform operations on the user's cart. Usually,
 * a service uses one or multiple repositories which encapsulate database access and perform such
 * operations but for the sake of simplicity, this project only uses services that do all the work.
 *
 * See https://martinfowler.com/eaaCatalog/repository.html
 * See https://pmichaels.net/service-repository-pattern
 */
using Dapper;
using ShopApi.Data;
using ShopApi.Utilities;

namespace ShopApi.Services;

// The interface is required since the service is used via dependency injection. Feel free to place
// interfaces in their own respective files but for the sake of simplicity, the interface is placed
// along with its implementation.
public interface ICartService
{
  Task<IEnumerable<CartPosition>> GetCartPositionsAsync(string userId, IEnumerable<int>? ids,
    CancellationToken cancellationToken);

  Task<int> UpsertCartPositionAsync(string userId, int productId, int? quantity,
    CancellationToken cancellationToken);

  Task DeleteCartPositionsAsync(string userId, IEnumerable<int>? ids,
    CancellationToken cancellationToken);

  Task<IEnumerable<int>> CreateOrderPositionsFromCartPositionsAsync(string userId, int orderId,
    IEnumerable<int>? ids, CancellationToken cancellationToken);
}

public class CartService : ICartService
{
  private readonly IAppDbContextFactory _appDbContextFactory;

  public CartService(IAppDbContextFactory appDbContextFactory)
    => _appDbContextFactory = appDbContextFactory;

  /// <summary>
  /// Get a collection of cart positions from a user.
  /// </summary>
  public async Task<IEnumerable<CartPosition>> GetCartPositionsAsync(string userId,
    IEnumerable<int>? ids, CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    var sql = $@"
      SELECT Id, UserId, ProductId, Quantity
      FROM Cart
      WHERE UserId = @UserId
      {(!ids.IsNullOrEmpty() ? "AND Id IN @Ids" : "")}";

    return await context.Connection.QueryAsync<CartPosition>(new CommandDefinition(sql, new
    {
      UserId = userId,
      Ids = ids
    }, context.Transaction, cancellationToken: cancellationToken));
  }

  /// <summary>
  /// Insert a new cart position or add to the quantity if the product already exists as a position
  /// in the cart of the user.
  /// </summary>
  public async Task<int> UpsertCartPositionAsync(string userId, int productId, int? quantity,
    CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    const string sql = @"
      INSERT INTO Cart(UserId, ProductId, Quantity)
      VALUES (@UserId, @ProductId, @Quantity)
      ON CONFLICT(UserId, ProductId)
      DO UPDATE SET Quantity = Quantity + @Quantity
      RETURNING Id";

    return await context.Connection.QuerySingleAsync<int>(new CommandDefinition(sql, new
    {
      UserId = userId,
      ProductId = productId,
      Quantity = quantity ?? 1
    }, context.Transaction, cancellationToken: cancellationToken));
  }

  /// <summary>
  /// Delete cart positions by IDs.
  /// </summary>
  public async Task DeleteCartPositionsAsync(string userId, IEnumerable<int>? ids,
    CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    var sql = $@"
      DELETE FROM Cart
      WHERE UserId = @UserId {(!ids.IsNullOrEmpty() ? "AND Id IN @Ids" : "")}";

    await context.Connection.ExecuteAsync(new CommandDefinition(sql, new
    {
      UserId = userId,
      Ids = ids
    }, context.Transaction, cancellationToken: cancellationToken));
  }

  public async Task<IEnumerable<int>> CreateOrderPositionsFromCartPositionsAsync(string userId,
    int orderId, IEnumerable<int>? ids, CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    var sql = $@"
      INSERT INTO OrderPosition (OrderId, ProductId, Quantity, Price)
      SELECT
        @OrderId,
        Cart.ProductId,
        Cart.Quantity,
        Product.Price
      FROM Cart
      JOIN Product ON Product.Id = Cart.ProductId
      WHERE Cart.UserId = @UserId {(!ids.IsNullOrEmpty() ? "AND Cart.Id IN @Ids" : "")}
      RETURNING OrderPosition.Id;";

    return await context.Connection.QueryAsync<int>(new CommandDefinition(sql, new
    {
      OrderId = orderId,
      UserId = userId,
      Ids = ids
    }, context.Transaction, cancellationToken: cancellationToken));
  }
}
