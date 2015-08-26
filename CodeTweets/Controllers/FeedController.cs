using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
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
        private ApplicationDbContext db = new ApplicationDbContext();
        private static bool toggleDate = true;
        private static bool toggleUser = true;

        // GET: Feed
        [Authorize]
        public ActionResult Index()
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            ViewBag.username = currentUser.user;

            List<CodePost> list = new List<CodePost>();

            if (currentUser.followList != null)
                list = getFollowedPosts(currentUser);
            else
                return View(list);

            return View(list);
        }

        private List<CodePost> getFollowedPosts(ApplicationUser currentUser)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                List<CodePost> posts = new List<CodePost>();

                foreach (var user in currentUser.followList)
                {
                    foreach (var post in context.posts)
                    {
                        if (post.user_id == user.Id)
                        {
                            posts.Add(post);
                        }
                    }
                }

                return posts;
            }
        }

        [HttpPost]
        public ContentResult getUserPosts(string orderParam, string type, string count, string hashtag)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser.user != null)
                ViewBag.username = currentUser.user;

            IEnumerable<CodePost> jsonList = null;

            int cnt = 0;

            if (!String.IsNullOrEmpty(count))
                cnt = Int32.Parse(count);

            //posts of the user
            // if (String.IsNullOrEmpty(type))
            /*jsonList = from code in db.posts.ToList()
                       where code.user_id == currentUser.Id
                       select code;*/


            if(!String.IsNullOrEmpty(hashtag))
            {
                jsonList = db.hashTags.
                       Where(t => t.Hash.tag == (hashtag)).
                       Select(t => t.Post).
                       ToList();
            }
            else
                jsonList = db.posts.ToList().Where(post => post.user_id == currentUser.Id);//.Take(5).Skip(cnt);
          //  else
            //    jsonList = getFollowedPosts(currentUser);

            if(!String.IsNullOrEmpty(orderParam))
            if(orderParam.Equals("date"))
            {
                    if (toggleDate)
                    {
                        jsonList = jsonList.OrderByDescending(post => post.date);
                        toggleDate = false;
                    }
                    else
                    {
                        jsonList = jsonList.OrderBy(post => post.date);
                        toggleDate = true;
                    }
                        
            }
            else if(orderParam.Equals("user"))
            {
                    if (toggleUser)
                    {
                        jsonList = jsonList.OrderByDescending(post => post.userName);
                        toggleUser = false;
                    }
                    else
                    {
                        jsonList = jsonList.OrderBy(post => post.userName);
                        toggleUser = true;
                    }
            }

            var list = JsonConvert.SerializeObject(jsonList,
                                                  Formatting.None,
                                                  new JsonSerializerSettings()
                                                  {
                                                      ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                  });

            return Content(list, "application/json");
        }

        //Izlistava sve postove trenutnog korisnika
        [Authorize]
        public ActionResult UserPosts(string hashtag)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            ViewBag.username = currentUser.user;

            IEnumerable<CodePost> list = null;

            if (hashtag != null)
            {
                HashTag searchtag = db.tags.ToList().Find(t => t.tag == hashtag);
                list = db.hashTags.
                       Where(t => t.Hash.tag == (hashtag)).
                       Select(t => t.Post).
                       ToList();
            }
            else
            {
                list = from code in db.posts.ToList()
                       where code.user_id == currentUser.Id
                       select code;

            }

            return View(list);
        }

        //vratimo od datog korisnika sve njegove postove
        [Authorize]
        public ActionResult UserPage(string userId)
        {

            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var viewedUser = manager.FindById(userId);

            ViewBag.banned = false;

            //check if the current user is banned from viewing this users homepage
            var banned = viewedUser.blockedList.Find(user => user.Id == currentUser.Id);

            IEnumerable<CodePost> list = null;

            //you are in their banned list
            if (banned != null)
            {
                ViewBag.banned = true;
                return View(list);
            }

             list = from code in db.posts.ToList()
                                         where code.user_id == userId
                                         select code;

            return View(list);
        }

        [HttpPost]
        public string AddFollow(string userId, string type)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var followedUser = manager.FindById(userId);
           
            if(type.Equals("Follow"))
            {
                currentUser.followList.Add(followedUser);
            }
            else
            {
                UnFollow(currentUser, followedUser);
            }

            manager.UpdateAsync(currentUser);

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

            store.Context.SaveChanges();
            db.SaveChanges();
            return "success";
        }

        public void UnFollow(ApplicationUser curr, ApplicationUser followed)
        {
            var follow = curr.followList.Find(user => user.Id == followed.Id);

            if (follow != null)
            {
                curr.followList.Remove(followed);
            }
        }

        [HttpPost]
        public string AddBlock(string userId, string type)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var blockedUser = manager.FindById(userId);

            if (type.Equals("Block"))
            {
                currentUser.blockedList.Add(blockedUser);

                //unfollow each person if there was any following
                UnFollow(currentUser, blockedUser);
                UnFollow(blockedUser, currentUser);
            }
            else  //unblock user just remove him from the list
            {
                var block = currentUser.blockedList.Find(user => user.Id == blockedUser.Id);

                if (block != null)
                {
                    currentUser.blockedList.Remove(block);
                }
            }

            manager.UpdateAsync(currentUser);

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

            store.Context.SaveChanges();
            db.SaveChanges();
            return "success";
        }


        //Like is called by angular to update like count on post
        [HttpPost]
        public string Like(int id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            foreach (CodePost post in db.posts.ToList())
            {
                if(post.id == id)
                {

                    var res = currentUser.usersVotes.Find(user => user.UserId == currentUser.Id && post.id == user.CodePostId);

                    if(res == null)
                    {
                        db.votes.Add(new UsersVotes() { User = currentUser, Post = post });

                        post.like++;
                        db.SaveChanges();
                        return "success";
                    }
                        
                    
                }
            }

            return "fail";
        }

        //Like is called by angular to update hate count on post
        [HttpPost]
        public string Hate(int id)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            foreach (CodePost post in db.posts.ToList())
            {
                if (post.id == id)
                {
                    var res = currentUser.usersVotes.Find(user => user.UserId == currentUser.Id && post.id == user.CodePostId);

                    if (res == null)
                    {
                        db.votes.Add(new UsersVotes() { User = currentUser, Post = post });

                        post.hate++;
                        db.SaveChanges();
                        return "success";
                    }
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

                currPost.content = "<a href='/Feed/UserPage?userId=" + currentUser.Id + "'> @" + currPost.userName + " </a> has tweeted: " + currPost.content;

                currPost.userName = currentUser.user;

                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

                //add the new post with the new id/name and handle
                db.posts.Add(currPost);

                store.Context.SaveChanges();

                return "success";
            }

            return "fail";
        }
    }
}