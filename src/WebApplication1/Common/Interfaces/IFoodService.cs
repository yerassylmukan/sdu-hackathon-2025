using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface IFoodService
{
    Task<Result<IEnumerable<FoodModel>>> GetFoodsAsync();
    Task<Result<FoodModel>> GetFoodAsync(Guid id);
    Task<Result<FoodModel>> CreateFoodAsync(RequestFoodModel request);
    Task<Result<FoodModel>> UpdateFoodAsync(Guid id, UpdateRequestFoodModel request);
    Task<Result> DeleteFoodAsync(Guid id);
}