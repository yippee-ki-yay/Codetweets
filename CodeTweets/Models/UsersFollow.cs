﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class UsersFollow
    {
        public int id { get; set; }
        public string UserFollowedId { get; set; }
        public string UserFollowingId { get; set; }
    }
}