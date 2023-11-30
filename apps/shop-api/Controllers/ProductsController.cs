using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IProductService _productService;

  public ProductsController(IProductService productService)
    => _productService = productService;

  [HttpGet("")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [Produces("application/json")]
  public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync(CancellationToken cancellationToken)
    => Ok(await _productService.GetProductsAsync(cancellationToken));
}
