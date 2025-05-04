namespace WebApplication1.Entities;

public class BasketItem
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public Basket Basket { get; set; }

    public Guid FoodId { get; set; }
    public Food Food { get; set; }

    public int Quantity { get; set; }
}