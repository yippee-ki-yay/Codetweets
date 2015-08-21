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
        CodePostDbContext db = new CodePostDbContext();

        public ActionResult One()
        {
            return View(db.posts.ToList());
        }

        public ActionResult Two()
        {
            return View();
        }

        public ActionResult Three()
        {
            return View();
        }

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
        private void updateHashTags(CodePost post)
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
            foreach(string postTag in tags)
            {
                HashTag result = db.tags.ToList().Find(t => t.tag == postTag);

                //this is a new tag
                if(result == null)
                {
                    HashTag newTag = new HashTag();
                    newTag.count = 0;
                    newTag.tag = postTag;

                    db.hashTags.Add(new HashTagPost(){ Hash = newTag, Post = post});
                    continue;
                }

                result.count++;
                db.hashTags.Add(new HashTagPost() { Hash = result, Post = post});
            }

           // db.SaveChanges();

        }

        [HttpPost]
        public string submitCodePost(string title, string content, string type)
        {
            CodePost tmp = new CodePost();

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            
            if(user.posts == null)
            {
                user.posts = new CodePost();
            }

            if (title != null && content != null && type != null)
            {
                tmp.title = title;
                tmp.content = content;
                tmp.votes = 0;
                tmp.user_id = user.Id;
                tmp.userName = user.user;
            }

            updateHashTags(tmp);

            user.posts = tmp;

           // IdentityDbContext db = new IdentityDbContext();
           // db.SaveChanges();

     //       db.posts.Add(tmp);
            db.SaveChanges();

            return "success";
        }
    }
}