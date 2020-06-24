using System;
using System.Collections.Generic;

namespace Microtransection.DB
{
    public partial class Users
    {
        public Users()
        {
            UserDetails = new HashSet<UserDetails>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? No { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<UserDetails> UserDetails { get; set; }
    }
}
