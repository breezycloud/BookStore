using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BookStore.Models
{
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public Guid Roleid { get; set; }
        public string Roletype { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
