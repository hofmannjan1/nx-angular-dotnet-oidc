/*
 * ABOUT THIS FILE
 *
 * This file is part of a basic implementation of the unit of work pattern for Dapper inspired by Nathan Cooper. The
 * unit of work creates a database context along with a transaction which is used for all commands within the unit of
 * work. It provides methods to either commit or roll back the changes and close the unit of work.
 *
 * Inspired by https://nathancooper.dev/articles/2020-03/unit-of-work-pattern
 */
using Microsoft.Data.Sqlite;

namespace ShopApi.Data;

public interface IUnitOfWork : IDisposable
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

  public UnitOfWork(string connectionString)
  {
    Context = new AppDbContext(connectionString, true);
  }

  public async Task BeginAsync()
  {
    Context.Transaction = await Context.Connection.BeginTransactionAsync() as SqliteTransaction;
  }

  public async Task RollbackAsync()
  {
    await Context.Transaction!.RollbackAsync();
  }

  public async Task CommitAsync()
  {
    await Context.Transaction!.CommitAsync();
  }

  public void Dispose()
  {
    Context.Connection?.Dispose();
    Context.Transaction?.Dispose();

    Context.IsDisposed = true;
  }
}
