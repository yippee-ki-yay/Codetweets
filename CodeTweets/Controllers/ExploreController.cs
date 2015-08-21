using CodeTweets.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace CodeTweets.Controllers
{
    public class ExploreController : Controller
    {
        //TODO: don't list the users that you already follow 
        // GET: Explore
        [Authorize]
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            var UsersContext = new ApplicationDbContext();

            IEnumerable<ApplicationUser> list = UsersContext.Users.Where(x => x.Id != userId).ToList();

            return View(list);
        }

        [HttpPost]
        public string UsersJson()
        {
            string userId = User.Identity.GetUserId();

            var UsersContext = new ApplicationDbContext();

            IEnumerable<ApplicationUser> list = UsersContext.Users.Where(x => x.Id != userId).ToList();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);

            return json;
        }

        [HttpPost]
        public string UsersSearchJson(string searchText)
        {
            string userId = User.Identity.GetUserId();

            var UsersContext = new ApplicationDbContext();

            IEnumerable<ApplicationUser> list = UsersContext.Users.Where(x => x.Id != userId && x.user.Contains(searchText)).ToList();

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(list);

            return json;
        }
    }
}