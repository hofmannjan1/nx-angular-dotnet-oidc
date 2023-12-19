/*
 * ABOUT THIS FILE
 *
 * This file is part of a basic implementation of the unit of work pattern for Dapper inspired by
 * Nathan Cooper. The factory provides methods to create a database context or an unit of work. If
 * there is an active unit of work, it will return the database context from the unit of work,
 * including the transaction. Otherwise, it will return a new database context without transaction.
 *
 * Inspired by https://nathancooper.dev/articles/2020-03/unit-of-work-pattern
 * See https://github.com/hofmannjan1/dapper-unit-of-work
 */
using Microsoft.Data.Sqlite;

namespace ShopApi.Data;

public interface IAppDbContextFactory
{
  IAppDbContext CreateContext();
  IUnitOfWork CreateUnitOfWork();
}

public class AppDbContextFactory : IAppDbContextFactory
{
  private readonly string? _connectionString;
  private IUnitOfWork? _unitOfWork;

  private bool IsUnitOfWorkActive => _unitOfWork is not null && !_unitOfWork.IsDisposed;

  public AppDbContextFactory(IConfiguration configuration) => 
    _connectionString = configuration.GetConnectionString("Sqlite");

  public IAppDbContext CreateContext() =>
    IsUnitOfWorkActive ? _unitOfWork!.Context : new AppDbContext(new SqliteConnection(_connectionString));

  public IUnitOfWork CreateUnitOfWork()
  {
    if (IsUnitOfWorkActive)
      throw new InvalidOperationException(
        "Could not begin a unit of work because there already exist an active unit of work.");

    return _unitOfWork = new UnitOfWork(new SqliteConnection(_connectionString));
  }
}
