namespace WebApplication1.Models;

public class OrderModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public string Status { get; set; }
    public List<OrderItemModel> Items { get; set; }
}