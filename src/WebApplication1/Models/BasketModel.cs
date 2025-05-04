namespace WebApplication1.Models;

public class BasketModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public List<BasketItemModel> Items { get; set; }
}