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

            List<CodePost> list = new List<CodePost>();

            List<string> followList = null;

            if (currentUser.followList != null)
                followList = currentUser.followList.Split('\n').ToList();
            else
                return View(list);

            foreach(CodePost post in db.posts.ToList())
            {
                if(followList.Contains(post.user_id))
                {
                    list.Add(post);
                }
            }

            return View(list);
        }

        [Authorize]
        public ActionResult UserPage()
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


            currentUser.followList += userId + '\n';

            manager.UpdateAsync(currentUser);

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

            store.Context.SaveChanges();
            return "success";
        }
    }
}