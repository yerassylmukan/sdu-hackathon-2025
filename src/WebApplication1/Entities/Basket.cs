namespace WebApplication1.Entities;

public class Basket
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public List<BasketItem> Items { get; set; } = new();
}