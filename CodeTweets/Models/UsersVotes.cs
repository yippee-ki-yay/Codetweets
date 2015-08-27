using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class UsersVotes
    {
        [Key, Column(Order = 0)]
        public int CodePostId { get; set; }

        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        public int Type { get; set; }

        public virtual CodePost Post { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}