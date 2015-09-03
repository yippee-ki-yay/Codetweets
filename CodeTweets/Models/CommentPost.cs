using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class CommentPost
    {
        [Key, Column(Order = 0)]
        public int CodePostId { get; set; }

        [Key, Column(Order = 1)]
        public int CommentId { get; set; }

        public CodePost Post { get; set; }

        public Comment Comment { get; set; }
    }
}