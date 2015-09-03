using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CodeTweets.Models
{
    public class Comment
    {
        public Comment()
        {
            date = DateTime.Now;
        }

        public int id { get; set; }
        public int post_id { get; set; }
        public string content { get; set; }
        public DateTime date { get; set; }
        public string user_id { get; set; }
        public string userName { get; set; }

        public IEnumerable<CommentPost> postComments { get; set; }

    }

}