/*
 * ABOUT THIS FILE
 *
 * This file is part of a basic implementation of the unit of work pattern for Dapper inspired by
 * Nathan Cooper. The key point here is that the database context knows if it's used within a unit
 * of work. If that's the case, it cannot be disposed on its own but the unit of work becomes
 * responsible of disposal. Otherwise, it would be automatically disposed after the first command
 * and could not be re-used for all commands within the unit of work. If the database context is
 * used outside an unit of work, it is disposed on its own.
 *
 * Inspired by https://nathancooper.dev/articles/2020-03/unit-of-work-pattern
 * See https://github.com/hofmannjan1/dapper-unit-of-work
 */
using System.Data.Common;

namespace ShopApi.Data;

public interface IAppDbContext : IAsyncDisposable
{
  public DbConnection Connection { get; }
  public DbTransaction? Transaction { get; set; }
  bool IsDisposed { get; set; }
}

public class AppDbContext : IAppDbContext
{
  public DbConnection Connection { get; }
  public DbTransaction? Transaction { get; set; }
  public bool IsDisposed { get; set; }

  private readonly bool _isUnitOfWorkContext;

  public AppDbContext(DbConnection connection, bool isUnitOfWorkContext = false)
  {
    Connection = connection;
    Connection.Open();

    _isUnitOfWorkContext = isUnitOfWorkContext;
  }
  
  public async ValueTask DisposeAsync()
  {
    // If there is an unit of work, let it take care of the disposal of the transaction/connection.
    if (_isUnitOfWorkContext)
      return;
    
    await Connection.DisposeAsync();
    if (Transaction is not null) 
      await Transaction.DisposeAsync();

    IsDisposed = true;
  }
}
