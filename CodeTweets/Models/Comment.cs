using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CodeTweets.Models
{
    public class Comment
    {
        public int id { get; set; }
        public string content { get; set; }

        public virtual CodePost opCode {get; set;}
    }

    public class CommentDbContext : DbContext
    {
        public DbSet<Comment> comments { get; set; }
    }

}