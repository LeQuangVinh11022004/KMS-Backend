using KMS.Service.DTOs.Notification;
using KMS.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace KMS.API.Hubs
{
    public class SignalRNotificationService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(int userId, NotificationDTO notification)
        {
            // Bắn event "ReceiveNotification" tới user có ID tương ứng
            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", notification);
        }
    }
}
