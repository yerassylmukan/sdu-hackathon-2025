using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class BasketService : IBasketService
{
    private readonly ApplicationDbContext _context;

    public BasketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BasketModel>> GetBasketAsync(string userId)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Food)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket == null)
            return Result<BasketModel>.Success(new BasketModel
            {
                UserId = userId,
                Items = new List<BasketItemModel>()
            });

        return Result<BasketModel>.Success(MapToBasketModel(basket));
    }

    public async Task<Result<BasketModel>> AddItemToBasketAsync(string userId, RequestBasketItemModel request)
    {
        var food = await _context.Foods.FindAsync(request.FoodId);
        if (food == null) return Result<BasketModel>.Failure("Food not found");

        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket == null)
        {
            basket = new Basket { Id = Guid.NewGuid(), UserId = userId };
            _context.Baskets.Add(basket);
        }

        var existingItem = basket.Items.FirstOrDefault(i => i.FoodId == request.FoodId);
        if (existingItem != null)
            existingItem.Quantity += request.Quantity;
        else
            basket.Items.Add(new BasketItem
            {
                Id = Guid.NewGuid(),
                FoodId = request.FoodId,
                Quantity = request.Quantity
            });

        await _context.SaveChangesAsync();

        return Result<BasketModel>.Success(MapToBasketModel(basket));
    }

    public async Task<Result> RemoveItemFromBasketAsync(Guid basketItemId)
    {
        var item = await _context.BasketItems.FindAsync(basketItemId);
        if (item == null) return Result.Failure("Basket item not found");

        _context.BasketItems.Remove(item);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> ClearBasketAsync(string userId)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket == null) return Result.Failure("Basket not found");

        _context.BasketItems.RemoveRange(basket.Items);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private BasketModel MapToBasketModel(Basket basket)
    {
        return new BasketModel
        {
            Id = basket.Id,
            UserId = basket.UserId,
            Items = basket.Items.Select(i => new BasketItemModel
            {
                Id = i.Id,
                FoodId = i.FoodId,
                FoodName = i.Food?.Name ?? "Unknown",
                Quantity = i.Quantity
            }).ToList()
        };
    }
}