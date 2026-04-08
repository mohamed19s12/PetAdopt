using Microsoft.AspNetCore.SignalR;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task SendNotificationAsync(string userId, string message)
        {
            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
