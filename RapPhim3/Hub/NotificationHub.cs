using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RapPhim3.Hubs
{
    public class NotificationHub : Hub
    {
        public static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User {userId} kết nối với ID {Context.ConnectionId}");
                ConnectedUsers[userId] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                ConnectedUsers.TryRemove(userId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string userId, string message)
        {
            Console.WriteLine($"Server gửi thông báo đến user {userId}: {message}");
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
    
}
