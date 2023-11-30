namespace ShopApi.Services;

public class Order
{
  public int Id { get; set; }
  public string UserId { get; set; }
  public string DateTime { get; set; }
  public decimal TotalPrice { get; set; }
}
