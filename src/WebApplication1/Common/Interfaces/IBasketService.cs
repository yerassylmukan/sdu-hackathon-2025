using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface IBasketService
{
    Task<Result<BasketModel>> GetBasketAsync(string userId);
    Task<Result<BasketModel>> AddItemToBasketAsync(string userId, RequestBasketItemModel request);
    Task<Result> RemoveItemFromBasketAsync(Guid basketItemId);
    Task<Result> ClearBasketAsync(string userId);
}