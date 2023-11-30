using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ShopApi.Contracts;
using ShopApi.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShopApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
  private readonly ICartService _cartService;

  public CartController(ICartService cartService)
    => _cartService = cartService;

  [HttpGet("positions")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [Produces("application/json")]
  public async Task<ActionResult<IEnumerable<CartPosition>>> GetCartPositionsAsync(
    CancellationToken cancellationToken)
  {
    var userId = User.GetClaim(Claims.Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    return Ok(await _cartService.GetCartPositionsAsync(userId, cancellationToken));
  }

  [HttpPost("positions")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [Consumes("application/json"), Produces("application/json")]
  // Use `SuppressInferBindingSourcesForParameters` to prevent .NET from inferring the FromBody
  // binding source for the CreateCartPositionRequest.
  public async Task<IActionResult> CreateCartPositionAsync(CreateCartPositionRequest request)
  {
    var userId = User.GetClaim(Claims.Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    var id = await _cartService.UpsertCartPositionAsync(userId, request.Position.ProductId,
      request.Position.Quantity, request.CancellationToken);

    return CreatedAtAction("GetCartPositions", new { id });
  }
}
