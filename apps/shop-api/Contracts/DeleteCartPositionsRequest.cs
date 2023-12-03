using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Contracts;

public class DeleteCartPositionsRequest
{
  [FromQuery] public IEnumerable<int> Ids { get; set; }
  public CancellationToken CancellationToken { get; set; }
}
