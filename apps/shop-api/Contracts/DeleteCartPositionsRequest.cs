namespace ShopApi.Contracts;

public class DeleteCartPositionsRequest
{
  public IEnumerable<int> Ids { get; set; }
  public CancellationToken CancellationToken { get; set; }
}
