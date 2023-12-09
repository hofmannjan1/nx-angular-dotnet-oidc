namespace ShopApi.Services;

public class CartPosition
{
  public int Id { get; set; }
  public string UserId { get; set; }
  public int ProductId { get; set; }
  public int Quantity { get; set; }
}
