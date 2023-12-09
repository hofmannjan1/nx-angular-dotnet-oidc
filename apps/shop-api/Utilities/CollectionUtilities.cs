using System.Collections;

namespace ShopApi.Utilities;

public static class CollectionUtilities
{
  public static bool IsNullOrEmpty(this IEnumerable? enumerable)
  {
    return enumerable is null || !enumerable.Cast<object?>().Any();
  }

  public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
  {
    return enumerable is null || !enumerable.Any();
  }
}
