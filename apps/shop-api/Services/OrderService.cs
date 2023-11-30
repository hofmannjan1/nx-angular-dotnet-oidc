using Dapper;
using ShopApi.Data;

namespace ShopApi.Services;

public interface IOrderService
{
  Task<IEnumerable<Order>> GetOrdersAsync(string userId, CancellationToken cancellationToken);
}

public class OrderService : IOrderService
{
  private readonly AppDbContext _appDbContext;

  public OrderService(AppDbContext appDbContext)
    => _appDbContext = appDbContext;

  public async Task<IEnumerable<Order>> GetOrdersAsync(string userId, CancellationToken cancellationToken)
  {
    using var dbConnection = _appDbContext.CreateConnection();

    const string sql = @"
      SELECT Id, UserId, [DateTime], ROUND((
          SELECT SUM(Price * Quantity)
          FROM OrderPosition
          WHERE OrderPosition.OrderId = [Order].Id
        ), 2) AS TotalPrice
      FROM [Order]
      WHERE UserId = @UserId";

    return await dbConnection.QueryAsync<Order>(new CommandDefinition(sql, new
    {
      UserId = userId
    }, cancellationToken: cancellationToken));
  }
}
