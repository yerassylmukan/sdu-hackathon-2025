﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<Result<BasketModel>> AddItemToBasketAsync(string userId, BasketItemRequestModel basketItemRequest)
    {
        var food = await _context.Foods.FindAsync(basketItemRequest.FoodId);
        if (food == null)
            return Result<BasketModel>.Failure("Food not found");

        var trackedBasket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (trackedBasket == null)
        {
            trackedBasket = new Basket
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Items = new List<BasketItem>()
            };
            _context.Baskets.Add(trackedBasket);
        }
        else
        {
            foreach (var item in trackedBasket.Items) _context.Entry(item).State = EntityState.Detached;

            trackedBasket.Items = await _context.BasketItems
                .Where(i => i.BasketId == trackedBasket.Id)
                .Include(i => i.Food)
                .ToListAsync();
        }

        var existingItem = trackedBasket.Items.FirstOrDefault(i => i.FoodId == basketItemRequest.FoodId);
        if (existingItem != null)
        {
            existingItem.Quantity += basketItemRequest.Quantity;
            _context.BasketItems.Update(existingItem);
        }
        else
        {
            var newItem = new BasketItem
            {
                Id = Guid.NewGuid(),
                BasketId = trackedBasket.Id,
                FoodId = basketItemRequest.FoodId,
                Quantity = basketItemRequest.Quantity
            };
            trackedBasket.Items.Add(newItem);
            _context.BasketItems.Add(newItem);
        }

        await _context.SaveChangesAsync();

        return Result<BasketModel>.Success(MapToBasketModel(trackedBasket));
    }

    public async Task<Result<int>> GetItemCountAsync(string userId)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket == null)
            return Result<int>.Success(0);

        var totalQuantity = basket.Items.Sum(i => i.Quantity);
        return Result<int>.Success(totalQuantity);
    }

    public async Task<Result<BasketModel>> IncreaseItemQuantityAsync(Guid basketItemId)
    {
        var item = await _context.BasketItems
            .Include(i => i.Food)
            .Include(i => i.Basket)
            .ThenInclude(b => b.Items)
            .ThenInclude(i => i.Food)
            .FirstOrDefaultAsync(i => i.Id == basketItemId);

        if (item == null)
            return Result<BasketModel>.Failure("Basket item not found");

        item.Quantity++;
        _context.BasketItems.Update(item);
        await _context.SaveChangesAsync();

        return Result<BasketModel>.Success(MapToBasketModel(item.Basket));
    }

    public async Task<Result<BasketModel>> DecreaseItemQuantityAsync(Guid basketItemId)
    {
        var item = await _context.BasketItems
            .Include(i => i.Food)
            .Include(i => i.Basket)
            .ThenInclude(b => b.Items)
            .ThenInclude(i => i.Food)
            .FirstOrDefaultAsync(i => i.Id == basketItemId);

        if (item == null)
            return Result<BasketModel>.Failure("Basket item not found");

        if (item.Quantity > 1)
        {
            item.Quantity--;
            _context.BasketItems.Update(item);
        }
        else
        {
            _context.BasketItems.Remove(item);
        }

        await _context.SaveChangesAsync();

        return Result<BasketModel>.Success(MapToBasketModel(item.Basket));
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

        basket.Items.Clear();

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
                FoodName = i.Food.Name,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}