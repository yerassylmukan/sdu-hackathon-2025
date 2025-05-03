using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs;

public class OrderStatusHub : Hub
{
    public async Task JoinOrderRoom(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
    }

    public async Task LeaveOrderRoom(string orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
    }
}