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

        public DateTime date { get; set; }

        public ApplicationUser fromUser { get; set; }
        public ApplicationUser toUser { get; set; }
    }
}