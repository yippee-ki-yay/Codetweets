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
        //returns a list of users you have exchanged messages with
        [HttpPost]
        public JsonResult getUsersITalkedTo()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser == null)
                return Json("");

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
                        var userN = dbList.Users.ToList().Find(usr => usr.Id == tmp.userId);
                        if(userN != null)
                            tmp.userName = userN.user;

                        tmp.unseenMsgCount = dbList.messages.Where(msg => msg.seen == false && tmp.userId == msg.toUserId && msg.fromUserId == currentUser.Id).Count();
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
                    var user = db.Users.ToList().Find(u => u.Id == tmp.fromId);
                    tmp.fromUser = user.user;
                    tmp.imgPath = user.userImgPath;
                    tmp.toUser = db.Users.ToList().Find(u => u.Id == tmp.toId).user;

                    tmp.msgState = (currentUser.Id == tmp.fromId) ? "CurrentUser" : "notCurrentUser";

                    resultList.Add(tmp);
                }
                    

            }

            return Json(resultList);
        }

        //gets the number of all the messages you havent seen
        [HttpPost]
        public int getUnseenMessageNumber()
        {

            using (ApplicationDbContext db =  new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser == null)
                    return 0; ;

                int sum = 0;

                sum = db.messages.Where(msg => msg.seen == false && msg.toUserId == currentUser.Id).Count();

                return sum;
            }

        }

        [HttpPost]
        public JsonResult getUnseenMessage()
        {


            return Json("");
        }

        //when the user has seen the messages change it's state
        [HttpPost]
        public int setMessagesSeen(string toUser)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                if (currentUser == null)
                    return 0;

                List<Message> unseenMessages = db.messages.Where( msg => msg.toUserId == currentUser.Id 
                                               && msg.fromUserId == toUser && msg.seen == false).ToList();

                //change the value of all set messages
                unseenMessages.All(m => { m.seen = true; return true; });

                db.SaveChanges();

                return unseenMessages.Count;
            }

           
        }
  
    }
}