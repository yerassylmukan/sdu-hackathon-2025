using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Common.Interfaces;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<OrderModel>> PlaceOrderFromBasketAsync(string userId)
    {
        var basket = await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (basket == null || !basket.Items.Any())
            return Result<OrderModel>.Failure("Basket is empty or not found");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Preparing,
            Items = basket.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                FoodId = i.FoodId,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.BasketItems.RemoveRange(basket.Items);
        await _context.SaveChangesAsync();

        return Result<OrderModel>.Success(MapToOrderModel(order));
    }

    public async Task<Result<IEnumerable<OrderModel>>> GetUserOrdersAsync(string userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Food)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Result<IEnumerable<OrderModel>>.Success(orders.Select(MapToOrderModel));
    }

    public async Task<Result<OrderModel>> GetOrderAsync(Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Food)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return Result<OrderModel>.Failure("Order not found");

        return Result<OrderModel>.Success(MapToOrderModel(order));
    }

    public async Task<Result> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return Result.Failure("Order not found");

        order.Status = newStatus;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<IEnumerable<OrderModel>>> GetOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Food)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Result<IEnumerable<OrderModel>>.Success(orders.Select(MapToOrderModel));

    }

    private OrderModel MapToOrderModel(Order order)
    {
        return new OrderModel
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            UserId = order.UserId,
            Status = order.Status.ToString(),
            Items = order.Items.Select(i => new OrderItemModel
            {
                Id = i.Id,
                FoodId = i.FoodId,
                FoodName = i.Food?.Name ?? "Unknown",
                Quantity = i.Quantity
            }).ToList()
        };
    }
}