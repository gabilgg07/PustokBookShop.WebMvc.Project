using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok
{
    public class ChatHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                user.ConnectionId = Context.ConnectionId;

                var result =  _userManager.UpdateAsync(user).Result;

                 Clients.All.SendAsync("LoadingPage", user.Id, user.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                user.ConnectionId = null;

                var result = _userManager.UpdateAsync(user).Result;
            }
            return base.OnDisconnectedAsync(exception);
        }

        //public async Task SendMessage(string fullname, string message, List<string> connectionIds)
        //{
        //    if (connectionIds.Count==1)
        //    {
        //        await Clients.All.SendAsync("ReceiveMessage", fullname, message);
        //    }
        //    else
        //    {
        //        await Clients.Clients(connectionIds).SendAsync("ReceiveMessage", fullname, message);
        //    }
           
        //}
    }
}
