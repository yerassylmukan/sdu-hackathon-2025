namespace WebApplication1.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}

public enum OrderStatus
{
    Preparing,
    Delivering,
    Delivered
}