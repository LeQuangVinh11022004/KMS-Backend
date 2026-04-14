using KMS.Service.DTOs.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMS.Service.Interfaces
{
    public interface INotificationHubService
    {
        Task SendNotificationToUserAsync(int userId, NotificationDTO notification);
    }
}
