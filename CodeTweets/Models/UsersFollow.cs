using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class UsersFollow
    {
        [Key, Column(Order = 0)]
        public int UserFollowId { get; set; }

        [Key, Column(Order = 1)]
        public int UserFollowedId { get; set; }

        public virtual ApplicationUser UserFollow { get; set; }

        public virtual ApplicationUser UserFollowed { get; set; }
    }
}