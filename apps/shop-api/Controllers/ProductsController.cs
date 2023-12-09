using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IProductsService _productsService;

  public ProductsController(IProductsService productsService) =>
      _productsService = productsService;

  [HttpGet("")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [Produces("application/json")]
  public async Task<ActionResult<IEnumerable<Product>>>
    GetProductsAsync(CancellationToken cancellationToken) =>
    Ok(await _productsService.GetProductsAsync(cancellationToken));
}
