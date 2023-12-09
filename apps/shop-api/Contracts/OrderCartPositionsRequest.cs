using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Contracts;

public class OrderCartPositionsRequest
{
  [FromQuery] public IEnumerable<int> Ids { get; set; }
  public CancellationToken CancellationToken { get; set; }
}
