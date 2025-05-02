using Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PresentationApi.Hubs;

namespace PresentationApi.SignalRServices
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<CustomerSupportHub> _hubContext;

        public SignalRNotificationService(IHubContext<CustomerSupportHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAdminsAsync(object message)
        {
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveSupportMessage", message);
        }

        public async Task NotifyUserAsync(string userId, object message)
        {
            await _hubContext.Clients.Group(userId).SendAsync("ReceiveAdminResponse", message);
        }
    }
}