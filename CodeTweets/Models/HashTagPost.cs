using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class HashTagPost
    {
        public System.Int32 CodePostId { get; set; }

        public System.Int32 HashTagId { get; set; }

        [ForeignKey("PostId")]
        public virtual CodePost Post { get; set; }

        [ForeignKey("CategoryId")]
        public virtual HashTag Hash { get; set; }

    }
}