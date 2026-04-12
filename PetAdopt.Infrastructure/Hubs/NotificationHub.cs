using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PetAdopt.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        //public override async Task OnConnectedAsync()
        //{
        //    //Catch the user
        //    var userId = Context.UserIdentifier;
        //    // Send a message to the caller with the userId
        //    await Clients.Caller.SendAsync("Connected",
        //        $"Connected successfully! UserId: {userId}");
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception? exception)
        //{
        //    await base.OnDisconnectedAsync(exception);
        //}
    }

}
