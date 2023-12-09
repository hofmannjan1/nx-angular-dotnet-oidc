/*
 * ABOUT THIS FILE
 *
 * This file is part of a basic implementation of the unit of work pattern for Dapper inspired by
 * Nathan Cooper. The factory provides methods to create a database context or an unit of work. If
 * there is an open unit of work, it will return the database context from the unit of work,
 * including the transaction. Otherwise, it will return a new database context without transaction.
 *
 * Inspired by https://nathancooper.dev/articles/2020-03/unit-of-work-pattern
 */
namespace ShopApi.Data;

public interface IAppDbContextFactory
{
  IAppDbContext CreateContext();
  IUnitOfWork CreateUnitOfWork();
}

public class AppDbContextFactory : IAppDbContextFactory
{
  private readonly string _connectionString;
  private IUnitOfWork? _unitOfWork;

  private bool IsUnitOfWorkOpen => _unitOfWork is not null && !_unitOfWork.IsDisposed;

  public AppDbContextFactory()
  {
    var tempPath = Path.Combine(Path.GetTempPath(), "nx-angular-dotnet-oidc");
    if (!File.Exists(tempPath))
      Directory.CreateDirectory(tempPath);

    _connectionString = $"Filename={Path.Combine(tempPath, "shop-api.sqlite3")}";
  }

  public IAppDbContext CreateContext() =>
    IsUnitOfWorkOpen ? _unitOfWork!.Context : new AppDbContext(_connectionString);

  public IUnitOfWork CreateUnitOfWork() =>
    IsUnitOfWorkOpen ? _unitOfWork! : _unitOfWork = new UnitOfWork(_connectionString);
}
