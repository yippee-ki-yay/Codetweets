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
        private static bool toggleDate = true;
        private static bool toggleUser = true;

        // GET: Feed
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("UserPosts");

        }

        private IEnumerable<CodePost> getFollowedPosts(ApplicationUser currentUser)
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

        private void orderPosts(string orderParam, ref List<ProfilePostsViewModel> jsonList)
        {
            if (orderParam.Equals("date"))
            {
                if (toggleDate)
                {
                    jsonList = jsonList.OrderByDescending(post => post.date).ToList();
                    toggleDate = false;
                }
                else
                {
                    jsonList = jsonList.OrderBy(post => post.date).ToList();
                    toggleDate = true;
                }

            }
            else if (orderParam.Equals("user"))
            {
                if (toggleUser)
                {
                    jsonList = jsonList.OrderByDescending(post => post.userName).ToList();
                    toggleUser = false;
                }
                else
                {
                    jsonList = jsonList.OrderBy(post => post.userName).ToList();
                    toggleUser = true;
                }
            }
        }

        [HttpPost]
        public JsonResult getUserPosts(string orderParam, string type, string count, string hashtag)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser.user != null)
                ViewBag.username = currentUser.user;

            IEnumerable<CodePost> jsonList = null;

            //number of posts for infinite scroll
            int cnt = 0;
            if (!String.IsNullOrEmpty(count))
                cnt = Int32.Parse(count);

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (!String.IsNullOrEmpty(hashtag)) //all posts with that hashtag
                {
                    jsonList = db.hashTags.
                           Where(t => t.Hash.tag == (hashtag)).
                           Select(t => t.Post).
                           ToList();
                }
                else if (type != null) //all posts from that user
                {
                    //this means ge the current user profile
                    if (type == "")
                        type = currentUser.Id;

                    jsonList = from code in db.posts.ToList()
                               where code.user_id == type
                               select code;
                }
                else //all posts of current users
                    jsonList = getFollowedPosts(currentUser);
            }
                


            var postArr = jsonList.Reverse().ToArray();

            //if no post found return empty
            if (postArr.Length == 0)
                return Json("");

            List<ProfilePostsViewModel> profileList = new List<ProfilePostsViewModel>();

            int increment = cnt;

            if (!String.IsNullOrEmpty(orderParam))
            {
                cnt = 0;
                increment = postArr.Length;
            }

            for (int i = cnt; i < postArr.Length; ++i)
            {
                if(i < increment+5)
                {
                    ProfilePostsViewModel tmp = new ProfilePostsViewModel();

                    CodePost post = postArr[i];

                    tmp.id = post.id;
                    tmp.title = post.title;
                    tmp.content = post.content;
                    tmp.like = post.like;
                    tmp.hate = post.hate;
                    tmp.userName = post.userName;
                    tmp.userImgPath = post.userImgPath;
                    tmp.date = post.date;
                    //uzmemo iz baze votova da li je korisnik na ovom postu lupio like/hate i saljemo ka klijentu


                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        var liked = (db.votes.ToList().Find(v => v.CodePostId == post.id && v.UserId == currentUser.Id && v.Type == 0));
                        var hated = (db.votes.ToList().Find(v => v.CodePostId == post.id && v.UserId == currentUser.Id && v.Type == 1));

                        tmp.liked = (liked != null) ? "You liked this" : "Like";
                        tmp.hated = (hated != null) ? "You hate this" : "Hate";

                       
                        //IEnumerable<CommentPost> commentPost 
                        tmp.commentList = from c in db.comments.ToList()
                                              where c.post_id == post.id
                                              select c;


                        profileList.Add(tmp);
                    }
                }
              
            }

            //Post sorting by date and user name
            if (!String.IsNullOrEmpty(orderParam))
                orderPosts(orderParam, ref profileList);

            return Json(profileList);
        }

        //Izlistava sve postove trenutnog korisnika
        [Authorize]
        public ActionResult UserPosts(string hashtag)
        {

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
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
        }

        //vratimo od datog korisnika sve njegove postove
        [Authorize]
        public ActionResult UserPage(string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
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
        }

        [HttpPost]
        public string AddFollow(string userId, string type)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var currentUser = manager.FindById(User.Identity.GetUserId());
                var followedUser = manager.FindById(userId);

                if (type.Equals("Follow"))
                {
                    currentUser.followList.Add(followedUser);
                }
                else
                {
                    UnFollow(currentUser, followedUser);
                }

               // manager.UpdateAsync(currentUser);

                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

                //store.Context.SaveChanges();
                db.SaveChanges();
                return "success";
            }
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
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
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

               // manager.UpdateAsync(currentUser);

                var store = new UserStore<ApplicationUser>(new ApplicationDbContext());

               // store.Context.SaveChanges();
                db.SaveChanges();
                return "success";
            }
        }


        //Like is called by angular to update like count on post
        [HttpPost]
        public string Like(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                var post = db.posts.ToList().Find(p => p.id == id);

                if (post != null)
                {

                    var res = currentUser.usersVotes.Find(user => user.UserId == currentUser.Id && post.id == user.CodePostId);

                    if (res == null)
                    {
                        db.votes.Add(new UsersVotes() { User = currentUser, Post = post, Type = 0 });

                        post.like++;
                        db.SaveChanges();
                        return "success";
                    }


                }


                return "fail";
            }
        }

        [HttpPost]
        public string Unlike(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                var post = db.posts.ToList().Find(p => p.id == id);

                if (post != null)
                {
                    var res = currentUser.usersVotes.Find(user => user.UserId == currentUser.Id && post.id == user.CodePostId);

                    if (res != null)
                    {
                        db.votes.Remove(res);
                        post.like--;
                        db.SaveChanges();

                        return "success";
                    }

                }



                return "fail";
            }
        }


        //Like is called by angular to update hate count on post
        [HttpPost]
        public string Hate(int id, string state)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var currentUser = manager.FindById(User.Identity.GetUserId());

                var post = db.posts.ToList().Find(p => p.id == id);

                if (post != null)
                {
                    var res = currentUser.usersVotes.Find(user => user.UserId == currentUser.Id && post.id == user.CodePostId);

                   
                        if (state.Equals("unhate"))
                        {
                             if (res != null)
                             {
                                 db.votes.Remove(res);
                                 post.hate--;
                            db.SaveChanges();
                            return "success";
                        }
                        }
                        else
                        {
                            if (res == null)
                            {
                                db.votes.Add(new UsersVotes() { User = currentUser, Post = post, Type = 1 });
                                 post.hate++;
                            db.SaveChanges();
                            return "success";
                        }
                        }

                    return "fail";
                    
                }


                return "fail";
            }
        }


        [HttpPost]
        public JsonResult commentsForPost(int postId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CodePost currPost = db.posts.ToList().Find(x => x.id == postId);

                if (currPost != null)
                {
                    var comments = db.postComments.Select(comment => comment.CodePostId == postId);



                    return Json(comments);
                }

                return Json("");
            }
        }

        [HttpPost]
        public JsonResult Reply(int postId, string commentContent)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //find the post
                CodePost currPost = db.posts.ToList().Find(x => x.id == postId);
                if (currPost != null)
                {
                    Comment newComment = new Comment() { content = commentContent, user_id = currPost.user_id, userName = currPost.userName, post_id = postId };

                    db.postComments.Add(new CommentPost() { Post = currPost, Comment = newComment });

                    db.SaveChanges();

                    return Json(newComment);
                }

                return Json("fail");
            }
        }

        //we take the post id you want to retweet and add to current user post list with @originalPoster inserted
        [HttpPost]
        public string Retweet(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //find the post
                CodePost currPost = db.posts.ToList().Find(x => x.id == id);

                if (currPost != null)
                {
                    var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    var currentUser = manager.FindById(User.Identity.GetUserId());

                    //currPost.user_id = currentUser.Id;

                    currPost.content = "<a href='/Feed/UserPosts?type=" + currPost.user_id + "'> @" + currPost.userName + " </a> has tweeted: " + currPost.content;

                    currPost.user_id = currentUser.Id;
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
}