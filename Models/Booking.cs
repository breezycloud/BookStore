using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BookStore.Models
{
    public partial class Booking
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? Bookid { get; set; }
        public Guid? Userid { get; set; }
        public string Status { get; set; }
        public DateTime? DateBooked { get; set; }
        public DateTime? DateReturned { get; set; }
        public string dateBooked => DateBooked.HasValue ? DateBooked.Value.ToShortDateString() : "";

        public string dateReturned => DateReturned.HasValue ? DateReturned.Value.ToShortDateString() : "";
        public virtual Book Book { get; set; }
        public virtual User User { get; set; }
    }
}
