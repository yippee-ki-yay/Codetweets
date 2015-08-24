using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class BlockedUsers
    {
        public int id { get; set; }

        public string FollowedUserId { get; set; }
        public string FollowsUserId { get; set; }
    }
}