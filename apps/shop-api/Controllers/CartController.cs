using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ShopApi.Contracts;
using ShopApi.Data;
using ShopApi.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShopApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
  private readonly ICartService _cartService;
  private readonly IOrdersService _ordersService;
  private readonly IAppDbContextFactory _appDbContextFactory;

  public CartController(ICartService cartService, IOrdersService ordersService,
    IAppDbContextFactory appDbContextFactory)
  {
    _cartService = cartService;
    _ordersService = ordersService;
    _appDbContextFactory = appDbContextFactory;
  }

  [HttpGet("positions")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [Produces("application/json")]
  public async Task<ActionResult<IEnumerable<CartPosition>>> GetCartPositionsAsync(
    CancellationToken cancellationToken)
  {
    var userId = User.GetClaim(Claims.Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    return Ok(await _cartService.GetCartPositionsAsync(userId, null, cancellationToken));
  }

  [HttpPost("positions")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [Consumes("application/json"), Produces("application/json")]
  public async Task<IActionResult> CreateCartPositionAsync(CreateCartPositionRequest request)
  {
    var userId = User.GetClaim(Claims.Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    var id = await _cartService.UpsertCartPositionAsync(userId, request.Position.ProductId,
      request.Position.Quantity, request.CancellationToken);

    return CreatedAtAction("GetCartPositions", new { id });
  }

  [HttpDelete("positions")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> DeleteCartPositionsAsync(DeleteCartPositionsRequest request)
  {
    var userId = User.GetClaim(Claims.Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    await _cartService.DeleteCartPositionsAsync(userId, request.Ids, request.CancellationToken);

    return Ok();
  }

  [HttpPost("positions/order")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> OrderCartPositionsAsync(OrderCartPositionsRequest request)
  {
    var userId = User.GetClaim(Claims.Subject)
      ?? throw new InvalidOperationException("Could not determine the user.");

    // A unit of work executes all commands between `BeginAsync()` and `CommitAsync()` in a single
    // transaction.
    using var unitOfWork = _appDbContextFactory.CreateUnitOfWork();

    try
    {
      // Begin the transaction.
      await unitOfWork.BeginAsync();

      // The AppDbContextFactory takes care if there is an active unit of work and provides its
      // database context with the same connection and transaction to the service. You don't have
      // to inject the transaction into the service methods.

      var orderId = await _ordersService.CreateEmptyOrderAsync(userId, request.CancellationToken);

      await _cartService.CreateOrderPositionsFromCartPositionsAsync(userId, orderId, request.Ids,
        request.CancellationToken);

      await _cartService.DeleteCartPositionsAsync(userId, request.Ids, request.CancellationToken);

      // Commit the changes if all commands are successful.
      await unitOfWork.CommitAsync();
    }
    catch (Exception)
    {
      // Roll back the changes if one of the commands throws an error.
      await unitOfWork.RollbackAsync();
      throw;
    }

    return Ok();
  }
}
