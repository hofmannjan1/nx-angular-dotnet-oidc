/*
 * ABOUT THIS FILE
 *
 * This file specifies a request contract between the client and the server to delete one or
 * multiple cart positions. Make sure to use `SuppressInferBindingSourcesForParameters` to prevent
 * .NET from inferring the FromBody binding source for request and instead consider the individual
 * binding source of each property.
 */
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Contracts;

public class DeleteCartPositionsRequest
{
  [FromQuery] public IEnumerable<int> Ids { get; set; }
  public CancellationToken CancellationToken { get; set; }
}
