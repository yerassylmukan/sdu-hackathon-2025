using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class FoodService : IFoodService
{
    private readonly ApplicationDbContext _context;

    public FoodService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<FoodModel>>> GetFoodsAsync()
    {
        var foods = await _context.Foods.ToListAsync();
        var foodModels = foods.Select(MapToFoodModel);
        return Result<IEnumerable<FoodModel>>.Success(foodModels);
    }

    public async Task<Result<FoodModel>> GetFoodAsync(Guid id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null) return Result<FoodModel>.Failure("Food not found");

        return Result<FoodModel>.Success(MapToFoodModel(food));
    }

    public async Task<Result<FoodModel>> CreateFoodAsync(RequestFoodModel request)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists) return Result<FoodModel>.Failure("Category does not exist");

        var food = new Food
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            PhotoUrl = request.PhotoUrl,
            Price = request.Price,
            PreparationTime = request.PreparationTime,
            Recipe = request.Recipe,
            CategoryId = request.CategoryId
        };

        _context.Foods.Add(food);
        await _context.SaveChangesAsync();

        return Result<FoodModel>.Success(MapToFoodModel(food));
    }

    public async Task<Result<FoodModel>> UpdateFoodAsync(Guid id, UpdateRequestFoodModel request)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null) return Result<FoodModel>.Failure("Food not found");

        if (request.Name is not null)
            food.Name = request.Name;

        if (request.Description is not null)
            food.Description = request.Description;

        if (request.PhotoUrl is not null)
            food.PhotoUrl = request.PhotoUrl;

        if (request.Price.HasValue)
            food.Price = request.Price.Value;

        if (request.PreparationTime.HasValue)
            food.PreparationTime = request.PreparationTime.Value;

        if (request.Recipe is not null)
            food.Recipe = request.Recipe;

        await _context.SaveChangesAsync();
        return Result<FoodModel>.Success(MapToFoodModel(food));
    }

    public async Task<Result> DeleteFoodAsync(Guid id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null) return Result.Failure("Food not found");

        _context.Foods.Remove(food);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private FoodModel MapToFoodModel(Food food)
    {
        return new FoodModel
        {
            Id = food.Id,
            Name = food.Name,
            Description = food.Description,
            PhotoUrl = food.PhotoUrl,
            Price = food.Price,
            PreparationTime = food.PreparationTime,
            Recipe = food.Recipe,
            CategoryId = food.CategoryId
        };
    }
}