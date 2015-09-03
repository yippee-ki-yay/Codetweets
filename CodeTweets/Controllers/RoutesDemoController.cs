using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Text.RegularExpressions;

namespace CodeTweets.Controllers
{
    public class RoutesDemoController : Controller
    { 
        [Authorize]
        public ActionResult addCodePost()
        {
            return View();
        }

        [HttpPost]
        public string ServerData()
        {
            return "server data";
        }

        //finds all the hashtags in the post, stores the new ones, updates the existing ones
        private bool updateHashTags(CodePost post)
        {
            bool added = false;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //extracts all the tags
                List<string> tags = new List<string>();

                var regex = new Regex(@"(?<=#)\w+");
                var matches = regex.Matches(post.content);

                foreach (Match m in matches)
                {
                    tags.Add(m.ToString());
                }

                //find and updates, not optimised should try O(1) access for every tag, for now this works
                foreach (string postTag in tags)
                {
                    HashTag result = db.tags.ToList().Find(t => t.tag == postTag);

                    //this is a new tag
                    if (result == null)
                    {
                        HashTag newTag = new HashTag();
                        newTag.count = 0;
                        newTag.tag = postTag;

                        db.posts.Add(post);

                        db.hashTags.Add(new HashTagPost() { Hash = newTag, Post = post });
                        added = true;
                        continue;
                    }

                    result.count++;
                    db.hashTags.Add(new HashTagPost() { Hash = result, Post = post });
                    db.posts.Add(post);
                    added = true;

                }

                db.SaveChanges();
            }

            return added;
        }

        [HttpPost]
        public string submitCodePost(string title, string content, string type)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                CodePost tmp = new CodePost();

                var store = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(store);
                ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
                var currentUser = userManager.FindById(User.Identity.GetUserId());

                if (title != null && content != null && type != null)
                {
                    tmp.title = title;
                    tmp.content = content;
                    tmp.votes = 0;
                    tmp.user_id = user.Id;
                    tmp.userName = user.user;
                    tmp.userImgPath = currentUser.userImgPath;
                }

                bool result = updateHashTags(tmp);
                
                if(result == false)
                    db.posts.Add(tmp);

                db.SaveChanges();

                return "success";
            }
                
        }
    }
}