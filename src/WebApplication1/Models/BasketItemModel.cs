namespace WebApplication1.Models;

public class BasketItemModel
{
    public Guid Id { get; set; }
    public Guid FoodId { get; set; }
    public string FoodName { get; set; }
    public int Quantity { get; set; }
}