using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace CodeTweets.Controllers
{
    public class ExploreController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Explore
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            var UsersContext = new ApplicationDbContext();

            IEnumerable<ApplicationUser> list = UsersContext.Users.Where(x => x.Id != userId).ToList();

            return View(list);
        }

        [HttpPost]
        public ContentResult UsersJson(string searchText, string userType)
        {
            string userId = User.Identity.GetUserId();

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            var UsersContext = new ApplicationDbContext();

            List<ApplicationUser> listUsers = null;

            if(searchText != null)
            {
                listUsers = UsersContext.Users.Where(x => x.Id != userId && x.user.Contains(searchText)).ToList();
            }
            else if(!String.IsNullOrEmpty(userType))
            {
                if(userType.Equals("All"))
                {
                    listUsers = UsersContext.Users.Where(x => x.Id != userId).ToList();
                }
                else if(userType.Equals("Following"))
                {
                    listUsers = currentUser.followList;
                }
                else if (userType.Equals("Followers"))
                {
                    List<ApplicationUser> tmpList = UsersContext.Users.ToList();

                    listUsers = new List<ApplicationUser>();

                    foreach (ApplicationUser user in tmpList)
                    {
                        var tmp = user.followList.Find(u => u.Id == currentUser.Id);

                        if (tmp != null)
                            listUsers.Add(user);
                    }
                }
                else if (userType.Equals("Blocked"))
                {
                    listUsers = currentUser.blockedList;
                }
            }
            else
                listUsers = UsersContext.Users.Where(x => x.Id != userId).ToList();

            //including additional information if the users are blocked/followed
            List<ExploreUsersViewModel> exploreList = new List<ExploreUsersViewModel>();

            foreach (var user in listUsers)
            {
                ExploreUsersViewModel tmp = new ExploreUsersViewModel();

                var follow = currentUser.followList.Find(u => u.Id == user.Id);
                var block = currentUser.blockedList.Find(u => u.Id == user.Id);

                tmp.isFollowed = (follow != null) ? "Unfollow" : "Follow";
                tmp.isBlocked = (block != null) ? "Unblock" : "Block";
                tmp.Id = user.Id;
                tmp.user = user.user;
                tmp.UserName = user.UserName;

                exploreList.Add(tmp);

            }

            var list = JsonConvert.SerializeObject(exploreList,
                                               Formatting.None,
                                               new JsonSerializerSettings()
                                               {
                                                   ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                               });
            

            return Content(list, "application/json");
        }

        [HttpPost]
        public string UsersSearchJson(string searchText)
        {
            string userId = User.Identity.GetUserId();

            var UsersContext = new ApplicationDbContext();

            IEnumerable<ApplicationUser> list = UsersContext.Users.Where(x => x.Id != userId && x.user.Contains(searchText)).ToList();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);

            return json;
        }
    }
}