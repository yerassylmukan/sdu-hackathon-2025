namespace WebApplication1.Models;

public class RequestFoodModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhotoUrl { get; set; }
    public decimal Price { get; set; }
    public TimeSpan PreparationTime { get; set; }
    public List<string> Recipe { get; set; }
    public Guid CategoryId { get; set; }
}