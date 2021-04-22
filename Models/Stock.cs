using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BookStore.Models
{
    public partial class Stock
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? Bookid { get; set; }
        public int? Total { get; set; }
        public int? Available { get; set; }

        public virtual Book Book { get; set; }
    }
}
