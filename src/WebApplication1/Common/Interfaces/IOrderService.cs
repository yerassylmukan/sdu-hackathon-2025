using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Common.Interfaces;

public interface IOrderService
{
    Task<Result<OrderModel>> PlaceOrderFromBasketAsync(string userId);
    Task<Result<IEnumerable<OrderModel>>> GetUserOrdersAsync(string userId);
    Task<Result<OrderModel>> GetOrderAsync(Guid orderId);
    Task<Result> CancelOrderAsync(Guid orderId);
    Task<Result> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<Result<IEnumerable<OrderModel>>> GetOrdersAsync();
}