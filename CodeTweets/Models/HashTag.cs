using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class HashTag
    {
        public int id { get; set; }
        public string tag { get; set; }
        public int count { get; set; }

        public virtual List<HashTagPost> tagPosts { get; set; }
    }


}