﻿using System;
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
        public string type { get; set; }
        public string user_id { get; set; }
        public string userName { get; set; }
        public int like { get; set; }
        public int hate { get; set; }

        public virtual List<HashTagPost> tagPosts { get; set; }
    }

    public class CodePostDbContext : DbContext
    {
        public DbSet<CodePost> posts { get; set; }
        public DbSet<HashTagPost> hashTags { get; set; }
    }
}