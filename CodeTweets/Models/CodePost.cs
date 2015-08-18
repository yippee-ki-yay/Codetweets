using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CodeTweets.Models
{
    public class CodePost
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int votes { get; set; }

    }

    public class CodePostDbContext : DbContext
    {
        public DbSet<CodePost> posts { get; set; }
    }
}