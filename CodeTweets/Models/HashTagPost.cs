﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CodeTweets.Models
{
    public class HashTagPost
    {
        [Key, Column(Order = 0)]
        public int CodePostId { get; set; }

        [Key, Column(Order = 1)]
        public int HashTagId { get; set; }
        
        public virtual CodePost Post { get; set; }

        public virtual HashTag Hash { get; set; }

    }
}