using Microsoft.AspNetCore.SignalR;

namespace APIGateway.Services
{
    public class NotificationHub : Hub
    {
        public async Task SubscribeToNotifications(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task UnsubscribeFromNotifications(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }
    }
}
