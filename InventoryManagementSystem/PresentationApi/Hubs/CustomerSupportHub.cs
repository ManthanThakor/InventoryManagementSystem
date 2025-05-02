using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace PresentationApi.Hubs
{
    [Authorize]
    public class CustomerSupportHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst("UserId")?.Value;
            var userType = Context.User.FindFirst("UserType")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                if (!string.IsNullOrEmpty(userType))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, userType);
                }

            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User.FindFirst("UserId")?.Value;



            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToAdmin(string message)
        {
            var userId = Context.User.FindFirst("UserId")?.Value;
            var username = Context.User.Identity?.Name;
            var userType = Context.User.FindFirst("UserType")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                throw new HubException("User identity not found");
            }

            await Clients.Group("Admin").SendAsync("ReceiveSupportMessage", new
            {
                userId,
                username,
                userType,
                message,
                timestamp = DateTime.UtcNow
            });

        }

        public async Task SendResponseToUser(string userId, string message)
        {
            if (!Context.User.IsInRole("Admin"))
            {
                throw new HubException("Only admins can send responses");
            }

            var adminName = Context.User.Identity?.Name;

            await Clients.Group(userId).SendAsync("ReceiveAdminResponse", new
            {
                adminName,
                message,
                timestamp = DateTime.UtcNow
            });

        }

        public async Task JoinSupportChannel(string channelName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
        }

        public async Task LeaveSupportChannel(string channelName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
        }
    }

}
