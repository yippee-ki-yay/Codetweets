using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CodeTweets.Models
{
    public class User
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        public virtual List<CodePost> posts {get; set;}
    }

    public class UserDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
    }

}