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
  private readonly AppDbContext _appDbContext;

  public OrdersService(AppDbContext appDbContext)
    => _appDbContext = appDbContext;

  public async Task<IEnumerable<Order>> GetOrdersAsync(string userId,
    CancellationToken cancellationToken)
  {
    using var connection = _appDbContext.CreateConnection();

    const string sql = @"
      SELECT Id, UserId, [DateTime], ROUND((
          SELECT SUM(Price * Quantity)
          FROM OrderPosition
          WHERE OrderPosition.OrderId = [Order].Id
        ), 2) AS TotalPrice
      FROM [Order]
      WHERE UserId = @UserId";

    return await connection.QueryAsync<Order>(new CommandDefinition(sql, new { UserId = userId },
      cancellationToken: cancellationToken));
  }

  public async Task<int> CreateEmptyOrderAsync(string userId, CancellationToken cancellationToken)
  {
    using var connection = _appDbContext.CreateConnection();

    const string sql = @"
      INSERT INTO [Order] (UserId, [DateTime])
      VALUES (@UserId, DATETIME('now'))
      RETURNING Id;";

    return await connection.QuerySingleAsync<int>(new CommandDefinition(sql, new { UserId = userId },
      cancellationToken: cancellationToken));
  }
}
