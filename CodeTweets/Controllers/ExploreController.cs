using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CodeTweets.Controllers
{
    public class ExploreController : Controller
    {
        // GET: Explore
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            var UsersContext = new ApplicationDbContext();

            IEnumerable<ApplicationUser> list = UsersContext.Users.Where(x => x.Id != userId).ToList();

            return View(list);
        }
    }
}