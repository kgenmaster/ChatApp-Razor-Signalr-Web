using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using ACE_MVC.DataService;

namespace ACE_MVC.ChatHub;

public class ChatHub : Hub
{
        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Request.Query["username"];
            Console.WriteLine($"User {username} has connected.");

            return base.OnConnectedAsync();
        }

        public async Task ConnectToGroup(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, username);
            System.Console.WriteLine($"{username} connected to the group.");
        }

        public override async Task OnDisconnectedAsync (Exception exception)
        {
            string userEmail = Context.User.Identity.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userEmail);
                Console.WriteLine($"{userEmail} disconnected.");
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage (string sender, string receiver, string message)
        {
            string senderHttpName = Context.GetHttpContext().Request.Query["username"];  // Retrieve sender username

            if (!string.IsNullOrEmpty(senderHttpName))
            {
                System.Console.WriteLine($"Sending message from {sender} to {receiver}: {message}");
                
                await Clients.Group(receiver).SendAsync("ReceiveMessage", sender, receiver, message);
                
                UserService service = new UserService ("Server=localhost;Database=chatapp;User=root;Password=free");
                service.SaveMessage (sender, receiver, message);
                
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", "System", "You are not authorized to send messages.");
            }
        }


}
