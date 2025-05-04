using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Common.Interfaces;
using WebApplication1.Entities;
using WebApplication1.Hubs;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrderController : ControllerBase
{
    private readonly IHubContext<OrderStatusHub> _hubContext;
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService, IHubContext<OrderStatusHub> hubContext)
    {
        _orderService = orderService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<ActionResult<OrderModel>> PlaceOrderFromBasket()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID is missing");

        var result = await _orderService.PlaceOrderFromBasketAsync(userId);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderModel>>> GetMyOrders()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID is missing");

        var result = await _orderService.GetUserOrdersAsync(userId);
        return Ok(result.Value);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderModel>> GetOrderById(Guid orderId)
    {
        var result = await _orderService.GetOrderAsync(orderId);
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, [FromQuery] OrderStatus newStatus)
    {
        var result = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
        if (result.IsFailure)
            return BadRequest(result.Error);

        await _hubContext.Clients
            .Group(orderId.ToString())
            .SendAsync("OrderStatusUpdated", newStatus.ToString());

        return Ok();
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OrderModel>>> GetOrders()
    {
        var result = await _orderService.GetOrdersAsync();
        return Ok(result.Value);
    }
}