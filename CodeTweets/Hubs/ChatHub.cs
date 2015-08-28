using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using CodeTweets.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodeTweets.Hubs
{
    public class ChatHub : Hub
    {
        static List<UserChatViewModel> chatUsers = new List<UserChatViewModel>();

        ApplicationDbContext db = new ApplicationDbContext();

        public void ConnectUser()
        {
            string username = Context.User.Identity.Name;

            var user = db.Users.ToList().Find(appUser => appUser.Email == username);

            if (user != null)
                username = user.user;

            var u = chatUsers.Find(ur => ur.userId == user.Id);

            if(u == null)
                chatUsers.Add(new UserChatViewModel() { userId = user.Id, chatId = Context.User.Identity.Name, name = user.user});
        }

        public void Send(string name, string message)
        {

            string username = Context.User.Identity.Name;


            var user = db.Users.ToList().Find(u => u.Email == username);

            if (user != null)
                username = user.user;

            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(username, message);
        }


        public void SendPrivateMessage(string message, string userId)
        {
            string username = Context.User.Identity.Name;
            var currentUser = db.Users.ToList().Find(usr => usr.Email == username);

            if (currentUser != null)
                username = currentUser.user;


           // Clients.Caller.sendPrivateMessage();

            var u = chatUsers.Find(user => user.userId == userId);

            if (u != null)
            {
                Clients.User(u.chatId).sendPrivateMessage(message, currentUser.user, "notCurrentUser");
                Clients.User(Context.User.Identity.Name).sendPrivateMessage(message, currentUser.user, "CurrentUser");
            }
        }
    }
}