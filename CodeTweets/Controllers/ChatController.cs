using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeTweets.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodeTweets.Controllers
{
    public class ChatController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        //returns a list of users you have exchanged messages with
        [HttpPost]
        public JsonResult getUsersITalkedTo()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            HashSet<UsersChatListViewModel> users = new HashSet<UsersChatListViewModel>();

            foreach (Message m in db.messages)
            {
                UsersChatListViewModel tmp = new UsersChatListViewModel();

                if(m.fromUserId == currentUser.Id || m.toUserId == currentUser.Id)
                {
                    if (m.fromUserId != currentUser.Id)
                        tmp.userId = m.fromUserId;
                    else if (m.toUserId != currentUser.Id)
                        tmp.userId = m.toUserId;



                    using (ApplicationDbContext dbList = new ApplicationDbContext())
                    {
                        tmp.userName = dbList.Users.ToList().Find(usr => usr.Id == tmp.userId).user;
                    }
                       
                    tmp.msgNumber = "0";
                    tmp.unreadMsg = "non";

                    users.Add(tmp);
                }
            }

            return Json(users);
        }

        //returns all the messages between currentUser and another
        [HttpPost]
        public JsonResult getChatConversation(string otherUserId)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            List<Message> messages = null;

            using (ApplicationDbContext db =  new ApplicationDbContext())
            {
                messages =
               db.messages.Where(msg => (msg.fromUserId == otherUserId && msg.toUserId == currentUser.Id)
                                      || (msg.fromUserId == currentUser.Id && msg.toUserId == otherUserId)).ToList();
            }

            List<ChatMessageViewModel> resultList = new List<ChatMessageViewModel>();

            foreach(Message m in messages)
            {
                ChatMessageViewModel tmp = new ChatMessageViewModel();


                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    tmp.fromId = m.fromUserId;
                    tmp.toId = m.toUserId;
                    tmp.content = m.content;
                    tmp.fromUser = db.Users.ToList().Find(u => u.Id == tmp.fromId).user;
                    tmp.toUser = db.Users.ToList().Find(u => u.Id == tmp.toId).user;

                    tmp.msgState = (currentUser.Id == tmp.fromId) ? "CurrentUser" : "notCurrentUser";

                    resultList.Add(tmp);
                }
                    

            }

            return Json(resultList);
        }


        [HttpPost]
        public int getUnseenMessageNumber()
        {
            int sum = 0;

            using (ApplicationDbContext db =  new ApplicationDbContext())
            {
                sum = db.messages.Where(msg => msg.seen == false).Count();
            }

            return sum;
        }

        [HttpPost]
        public JsonResult getUnseenMessage()
        {


            return Json("");
        }
  
    }
}