/*
 * ABOUT THIS FILE
 *
 * This file specifies a request contract between the client and the server to create a cart
 * position. Make sure to use `SuppressInferBindingSourcesForParameters` to prevent .NET from
 * inferring the FromBody binding source for request and instead consider the individual binding
 * source of each property.
 */
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
