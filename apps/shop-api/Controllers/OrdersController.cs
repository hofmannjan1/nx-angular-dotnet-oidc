using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ShopApi.Services;
using static OpenIddict.Abstractions.OpenIddictConstants.Claims;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
  private readonly IOrdersService _ordersService;

  public OrdersController(IOrdersService ordersService) =>
    _ordersService = ordersService;

  [HttpGet("")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [Produces("application/json")]
  public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAsync(
    CancellationToken cancellationToken)
  {
    var userId = User.GetClaim(Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    return Ok(await _ordersService.GetOrdersAsync(userId, cancellationToken));
  }
}
