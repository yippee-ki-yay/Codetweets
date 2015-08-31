using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class Message
    {
        public Message()
        {
            date = DateTime.Now;
        }

        public int id { get; set; }
        public string content { get; set; }

        public bool seen { get; set; }

        public DateTime date { get; set; }

        public string fromUserId { get; set; }
        public string toUserId { get; set; }
    }
}