using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface IFoodService
{
    Task<Result<IEnumerable<FoodModel>>> GetFoodsAsync();
    Task<Result<FoodModel>> GetFoodAsync(Guid id);
    Task<Result<IEnumerable<FoodModel>>> GetFoodsByCategoryIdAsync(Guid categoryId);
    Task<Result<IEnumerable<FoodModel>>> SearchFoodsAsync(string query);
    Task<Result<FoodModel>> CreateFoodAsync(FoodRequestModel foodRequest);
    Task<Result<FoodModel>> UpdateFoodAsync(Guid id, UpdateRequestFoodModel request);
    Task<Result> DeleteFoodAsync(Guid id);
}