using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface IBasketService
{
    Task<Result<BasketModel>> GetBasketAsync(string userId);
    Task<Result<BasketModel>> AddItemToBasketAsync(string userId, BasketItemRequestModel basketItemRequest);
    Task<Result<int>> GetItemCountAsync(string userId);
    Task<Result<BasketModel>> IncreaseItemQuantityAsync(Guid basketItemId);
    Task<Result<BasketModel>> DecreaseItemQuantityAsync(Guid basketItemId);
    Task<Result> RemoveItemFromBasketAsync(Guid basketItemId);
    Task<Result> ClearBasketAsync(string userId);
}