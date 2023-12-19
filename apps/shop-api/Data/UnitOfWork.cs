/*
 * ABOUT THIS FILE
 *
 * This file is part of a basic implementation of the unit of work pattern for Dapper inspired by
 * Nathan Cooper. The unit of work creates a database context along with a transaction which is
 * used for all commands within the unit of work. It provides methods to either commit or roll back
 * the changes and end the unit of work.
 *
 * Inspired by https://nathancooper.dev/articles/2020-03/unit-of-work-pattern
 * See https://github.com/hofmannjan1/dapper-unit-of-work
 */
using System.Data.Common;

namespace ShopApi.Data;

public interface IUnitOfWork : IAsyncDisposable
{
  AppDbContext Context { get; }
  bool IsDisposed { get; }
  Task BeginAsync();
  Task RollbackAsync();
  Task CommitAsync();
}

public class UnitOfWork : IUnitOfWork
{
  public AppDbContext Context { get; }
  public bool IsDisposed => Context.IsDisposed;

  public UnitOfWork(DbConnection connection) => Context = new AppDbContext(connection, true);

  public async Task BeginAsync() =>
    Context.Transaction = await Context.Connection.BeginTransactionAsync();

  public async Task RollbackAsync() => await Context.Transaction!.RollbackAsync();

  public async Task CommitAsync() => await Context.Transaction!.CommitAsync();
  
  public async ValueTask DisposeAsync()
  {
    await Context.Connection.DisposeAsync();
        if (Context.Transaction is not null) 
          await Context.Transaction.DisposeAsync();
    
        Context.IsDisposed = true;
  }
}
