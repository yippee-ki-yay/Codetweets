using CodeTweets.Models;
using System;
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

            var UsersContext = new ApplicationDbContext();

            return View(UsersContext.Users.ToList());
        }
    }
}