using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KMS.API.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // Mặc định SignalR tự động đọc ClaimTypes.NameIdentifier từ JWT Token
        // và map ConnectionId đó với UserId. Nên bạn không cần viết thêm hàm gì ở đây cả.
    }
}
