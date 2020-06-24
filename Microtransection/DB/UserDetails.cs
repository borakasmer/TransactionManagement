using System;
using System.Collections.Generic;

namespace Microtransection.DB
{
    public partial class UserDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
    }
}
