using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Contracts;

public class CreateCartPositionRequest
{
  public class CreateCartPositionDto
  {
    public int ProductId { get; set; }
    public int? Quantity { get; set; }
  }

  [FromBody] public CreateCartPositionDto Position { get; set; }
  public CancellationToken CancellationToken { get; set; }
}
