using Dapper;
using ShopApi.Data;

namespace ShopApi.Services;

public interface IOrdersService
{
  Task<IEnumerable<Order>> GetOrdersAsync(string userId, CancellationToken cancellationToken);

  Task<int> CreateEmptyOrderAsync(string userId, CancellationToken cancellationToken);
}

public class OrdersService : IOrdersService
{
  private readonly IAppDbContextFactory _appDbContextFactory;

  public OrdersService(IAppDbContextFactory appDbContextFactory)
    => _appDbContextFactory = appDbContextFactory;

  public async Task<IEnumerable<Order>> GetOrdersAsync(string userId,
    CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    const string sql = @"
      SELECT Id, UserId, [DateTime], ROUND((
          SELECT SUM(Price * Quantity)
          FROM OrderPosition
          WHERE OrderPosition.OrderId = [Order].Id
        ), 2) AS TotalPrice
      FROM [Order]
      WHERE UserId = @UserId";

    return await context.Connection.QueryAsync<Order>(new CommandDefinition(sql,
      new { UserId = userId }, context.Transaction, cancellationToken: cancellationToken));
  }

  public async Task<int> CreateEmptyOrderAsync(string userId, CancellationToken cancellationToken)
  {
    await using var context = _appDbContextFactory.CreateContext();

    const string sql = @"
      INSERT INTO [Order] (UserId, [DateTime])
      VALUES (@UserId, DATETIME('now'))
      RETURNING Id;";

    return await context.Connection.QuerySingleAsync<int>(new CommandDefinition(sql,
      new { UserId = userId }, context.Transaction, cancellationToken: cancellationToken));
  }
}
