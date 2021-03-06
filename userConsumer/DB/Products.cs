﻿using System;
using System.Collections.Generic;

namespace userConsumer.DB
{
    public partial class Products
    {
        public Products()
        {
            UserDetails = new HashSet<UserDetails>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<UserDetails> UserDetails { get; set; }
    }
}
