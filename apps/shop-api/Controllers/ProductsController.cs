using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
  private static readonly Product[] Products =
  {
    new() { Id = 1, Name = "Lagavulin 16 Year Old Single Malt Scotch Whisky", Price = (decimal) 69.9 },
    new() { Id = 2, Name = "Ardbeg 10 Year Old Single Malt Scotch Whisky", Price = (decimal) 39.9 },
    new() { Id = 3, Name = "Knob Creek 9 Year Old Kentucky Straight Bourbon Whiskey", Price = (decimal) 29.9 },
    new() { Id = 4, Name = "Wild Turkey Rare Breed Barrel Proof Kentucky Straight Bourbon Whiskey", Price = (decimal) 39.9 },
    new() { Id = 5, Name = "Montelobos Mezcal Espadín", Price = (decimal) 39.9 },
    new() { Id = 6, Name = "Marca Negra Mezcal Espadín", Price = (decimal) 59.9 }
  };

  [HttpGet("")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [Produces("application/json")]
  public ActionResult<IEnumerable<Product>> GetProducts() => Products;
}
