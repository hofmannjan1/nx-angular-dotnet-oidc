using System.Data;
using Microsoft.Data.Sqlite;

namespace ShopApi.Data;

public class AppDbContext
{
  public IDbConnection CreateConnection()
  {
    var tempPath = Path.Combine(Path.GetTempPath(), "nx-angular-dotnet-oidc");
    if (!File.Exists(tempPath))
      Directory.CreateDirectory(tempPath);

    return new SqliteConnection($"Filename={Path.Combine(tempPath, "shop-api.sqlite3")}");
  }
}
