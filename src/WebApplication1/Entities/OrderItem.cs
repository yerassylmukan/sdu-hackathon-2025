namespace WebApplication1.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public Guid FoodId { get; set; }
    public Food Food { get; set; }

    public int Quantity { get; set; }
}