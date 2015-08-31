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

        static ApplicationDbContext db = new ApplicationDbContext();

        public void ConnectUser()
        {
            string username = Context.User.Identity.Name;

            var user = db.Users.ToList().Find(appUser => appUser.Email == username);

            if (user == null)
                return;

            if (user != null)
                username = user.user;

            var u = chatUsers.Find(ur => ur.userId == user.Id);

            if (u == null)
                chatUsers.Add(new UserChatViewModel() { userId = user.Id, chatId = Context.User.Identity.Name, name = user.user, appUser = user});
        }

        public void isTyping(string userId)
        {
            var u = chatUsers.Find(user => user.userId == userId);

            Clients.User(u.chatId).showTyping(u.name);
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string username = Context.User.Identity.Name;
                var currentUser = db.Users.ToList().Find(usr => usr.Email == username);

                if (currentUser != null)
                    username = currentUser.user;

                //u is the user i'm sending to
                var u = chatUsers.Find(user => user.userId == userId);

                //other user Name
                string sendToUserName = db.Users.ToList().Find(usr => usr.Id == userId).user;

                if (u != null)
                {
                    db.messages.Add(new Message() { content = message, fromUserId = currentUser.Id, toUserId = u.appUser.Id, seen = true });

                    Clients.User(u.chatId).sendPrivateMessage(currentUser.Id, u.name, message, currentUser.user, "notCurrentUser");
                    Clients.User(Context.User.Identity.Name).sendPrivateMessage(currentUser.Id, u.name, message, currentUser.user, "CurrentUser");
                }
                else
                {
                    db.messages.Add(new Message() { content = message, fromUserId = currentUser.Id, toUserId = userId, seen = false });
                    Clients.User(Context.User.Identity.Name).sendPrivateMessage(currentUser.Id, sendToUserName, message, currentUser.user, "CurrentUser");
                }

                db.SaveChanges();
            }
        }
    }
}