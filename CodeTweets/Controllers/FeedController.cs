using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeTweets.Controllers
{
    public class FeedController : Controller
    {
        private CodePostDbContext db = new CodePostDbContext();

        // GET: Feed
        [Authorize]
        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            ViewBag.username = currentUser.user;

            IEnumerable<CodePost> list = from code in db.posts.ToList()
                                         where code.user_id == currentUser.Id
                                         select code;

            return View(list);
        }

        //vratimo od datog korisnika sve njegove postove
        [Authorize]
        public ActionResult UserPage(string userId)
        {

            IEnumerable<CodePost> list = from code in db.posts.ToList()
                                         where code.user_id == userId
                                         select code;

            return View(list);
        }

        [HttpPost]
        public string AddFollow(string userId)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if(currentUser.followList == null)
            {
                currentUser.followList = new List<string>();
            }

            currentUser.followList.Add(userId);

            return "success";
        }
    }
}