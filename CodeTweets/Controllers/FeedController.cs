using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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


        [HttpPost]
        public string getUserPosts()
        {
            /*var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if(currentUser.user != null)
            ViewBag.username = currentUser.user;

            IEnumerable<CodePost> list = from code in db.posts.ToList()
                                         where code.user_id == currentUser.Id
                                         select code;

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);

            return json;*/
            return "";
        }

        //Izlistava sve postove trenutnog korisnika
        [Authorize]
        public ActionResult UserPosts()
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

        //Like is called by angular to update like count on post
        [HttpPost]
        public string Like(int id)
        {
            foreach(CodePost post in db.posts.ToList())
            {
                if(post.id == id)
                {
                    post.like++;
                    db.SaveChanges();
                    return "success";
                }
            }

            return "fail";
        }

        //Like is called by angular to update hate count on post
        [HttpPost]
        public string Hate(int id)
        {
            foreach (CodePost post in db.posts.ToList())
            {
                if (post.id == id)
                {
                    post.hate++;
                    db.SaveChanges();
                    return "success";
                }
            }

            return "fail";
        }

        //we take the post id you want to retweet and add to current user post list with @originalPoster inserted
        [HttpPost]
        public string Retweet(int id)
        {
            //find the post
            CodePost currPost = db.posts.ToList().Find(x => x.id == id);

            if(currPost != null)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                currPost.user_id = currentUser.Id;

                currPost.content = "@" + currPost.userName + "has tweeted: " + currPost.content;

                currPost.userName = currentUser.user;
    

                //add the new post with the new id/name and handle
                db.posts.Add(currPost);

                db.SaveChanges();

                return "success";
            }

            return "fail";
        }
    }
}