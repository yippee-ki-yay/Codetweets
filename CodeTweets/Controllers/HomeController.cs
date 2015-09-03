using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CodeTweets.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Feed");
            }

            return View();
        }

        public ActionResult Messages()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public string GenerateFileName(string context)
        {
            return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
        }


        private void reloadImage(ApplicationUser currUser, string newPath)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                currUser.userImgPath = newPath;

                //find all the posts of this user and change it's userImgPath data
                List<CodePost> posts = db.posts.Where(post => post.user_id == currUser.Id).ToList();

                posts.All(p => { p.userImgPath = newPath; return true; });

                db.SaveChanges();
            }

        }

        [HttpPost]
        public void AddImage()
        {
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];

                string fileName = GenerateFileName("Image") +"."+ pic.FileName.Split('.')[1];

                pic.SaveAs(Server.MapPath("/img/" + fileName));

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var store = new UserStore<ApplicationUser>(db);
                    var manager = new UserManager<ApplicationUser>(store);
                    var currentUser = manager.FindById(User.Identity.GetUserId());

                    //if the user already has an image
                    if(currentUser.userImgPath != "")
                    {
                        reloadImage(currentUser, fileName);
                    }
                    else
                        currentUser.userImgPath = fileName;

                    db.SaveChanges();
                    // manager.UpdateAsync(currentUser);
                    //store.Context.SaveChanges();
                    // FormsAuthentication.SignOut();
                   // Session.Abandon();
                   // Index();
                }

            }
        }

    }
}