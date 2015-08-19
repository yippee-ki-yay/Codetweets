using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

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

        public ActionResult addCodePost()
        {
            return View();
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
                tmp.type = type;
                tmp.votes = 0;
                tmp.user_id = user.Id;
            }

            user.posts = tmp;

           // IdentityDbContext db = new IdentityDbContext();
           // db.SaveChanges();

            db.posts.Add(tmp);
            db.SaveChanges();

            return "success";
        }
    }
}