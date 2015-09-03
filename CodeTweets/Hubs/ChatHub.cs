using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using CodeTweets.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace CodeTweets.Hubs
{
    public class ChatHub : Hub
    {
        private static List<UserChatViewModel> chatUsers = new List<UserChatViewModel>();

        public void ConnectUser()
        {
            string username = Context.User.Identity.Name;

            ApplicationUser user = null;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                user = db.Users.ToList().Find(appUser => appUser.Email == username);
            }

            if (user == null)
                return;

            if (user != null)
                username = user.user;

            lock(chatUsers)
            {
                var u = chatUsers.Find(ur => ur.userId == user.Id);

                if (u == null)
                    chatUsers.Add(new UserChatViewModel()
                    {
                        userId = user.Id,
                        chatId = Context.User.Identity.Name,
                        name = user.user,
                        appUser = user,
                        userImagePath = user.userImgPath,
                        isAway = false
                    });
            }
           
        }

        //called by client to set if the user is inactive for a while
        public void UserAway()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                lock(chatUsers)
                {
                    var currChatUser = chatUsers.Find(u => u.chatId == Context.User.Identity.Name);

                    if (currChatUser != null)
                    {
                        currChatUser.isAway = true;
                    }
                }
            }
        }

        //called by client when the user is back online
        public void UserBackOnline()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                lock (chatUsers)
                {
                    var currChatUser = chatUsers.Find(u => u.chatId == Context.User.Identity.Name);

                    if (currChatUser != null)
                    {
                        currChatUser.isAway = false;
                    }
                }
            }
        }

        public void isTyping(string userId)
        {
            lock (chatUsers)
            {
                var u = chatUsers.Find(user => user.chatId == Context.User.Identity.Name);

                var otherUser = chatUsers.Find(user => user.userId == userId);

                if (u != null && otherUser != null)
                    Clients.User(otherUser.chatId).showTyping(u.name);
            }
        }

        public void notTyping(string userId)
        {
            lock (chatUsers)
            {
                var u = chatUsers.Find(user => user.chatId == Context.User.Identity.Name);

                var otherUser = chatUsers.Find(user => user.userId == userId);

                if (u != null && otherUser != null)
                    Clients.User(otherUser.chatId).removeTyping(u.name);
            }
        }

        //I don't this we are using this, something bad might happend if you deleted this
        public void Send(string name, string message)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string username = Context.User.Identity.Name;

                var user = db.Users.ToList().Find(u => u.Email == username);

                if (user != null)
                    username = user.user;

                // Call the addNewMessageToPage method to update clients.
                Clients.All.addNewMessageToPage(username, message);
            }
               
        }

        //client calls this to cheks the status of the user
        public string isConnected(string userId)
        {
            lock (chatUsers)
            {
                UserChatViewModel currUser = chatUsers.Find(user => user.userId == userId);

                if (currUser != null)
                    if (currUser.isAway)
                        return "Away";

                return (currUser == null) ? "Offline" : "Online";
            }
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            lock (chatUsers)
            {
                UserChatViewModel currUser = chatUsers.Find(user => user.chatId == Context.User.Identity.Name);

                if (currUser != null)
                    chatUsers.Remove(currUser);

                return base.OnDisconnected(stopCalled);
            }
        }

        public void SendPrivateMessage(string message, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string username = Context.User.Identity.Name;
                var currentUser = db.Users.ToList().Find(usr => usr.Email == username);

                if (currentUser != null)
                    username = currentUser.user;

                lock (chatUsers)
                {
                    //u is the user i'm sending to
                    var u = chatUsers.Find(user => user.userId == userId);

                    //other user Name
                    string sendToUserName = db.Users.ToList().Find(usr => usr.Id == userId).user;

                    if (u != null)
                    {
                        db.messages.Add(new Message() { content = message, fromUserId = currentUser.Id, toUserId = u.appUser.Id, seen = true });

                        Clients.User(u.chatId).sendPrivateMessage(currentUser.Id, u.name, message, currentUser.user, "notCurrentUser", currentUser.userImgPath);
                        Clients.User(Context.User.Identity.Name).sendPrivateMessage(currentUser.Id, u.name, message, currentUser.user, "CurrentUser", currentUser.userImgPath);
                    }
                    else
                    {
                        db.messages.Add(new Message() { content = message, fromUserId = currentUser.Id, toUserId = userId, seen = false });
                        Clients.User(Context.User.Identity.Name).sendPrivateMessage(currentUser.Id, sendToUserName, message, currentUser.user, "CurrentUser", currentUser.userImgPath);
                    }

                    db.SaveChanges();
                }
            }
        }
    }
}