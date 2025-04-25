using AllProductsService.Protos;
using Microsoft.AspNetCore.SignalR;

namespace APIGateway.Services
{
    public class NotificationService : IHostedService
    {
        private IHubContext<NotificationHub> hubContext;
        private RabbitMQService rabbitMQService;

        public NotificationService(IHubContext<NotificationHub> hubContext, RabbitMQService rabbitMQService)
        {
            this.hubContext = hubContext;
            this.rabbitMQService = rabbitMQService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            rabbitMQService.SubscribeToQueue("QueuePositionUpdates", async (data) =>
            {
                var queuePositionUpdate = QueuePositionUpdateMessage.Parser.ParseFrom(data);
                var updateMessage = queuePositionUpdate.UpdateMessage;
                await SendNotificationAsync(queuePositionUpdate.UserId, "QueuePositionUpdate", updateMessage);
            });

            rabbitMQService.SubscribeToQueue("LostProducts", async (data) =>
            {
                var lostProduct = LostProduct.Parser.ParseFrom(data);
                await SendNotificationAsync(lostProduct.UserId, "LostProduct", lostProduct.ProductId);
            });

            rabbitMQService.SubscribeToQueue("ProductStatusUpdates", async (data) =>
            {
                var productStatusUpdate = ProductStatusUpdate.Parser.ParseFrom(data);
                await SendNotificationAsync(productStatusUpdate.UserId, "ProductStatusUpdates", productStatusUpdate);
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task SendNotificationAsync(string userId, string notificationType, object payload)
        {
            await hubContext.Clients.Group(userId).SendAsync(notificationType, payload);
        }
    }
}
