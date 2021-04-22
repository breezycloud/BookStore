using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BookStore.Models
{
    public partial class Book
    {
        public Book()
        {
            Bookings = new HashSet<Booking>();
            Stocks = new HashSet<Stock>();
        }

        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public string Isbn { get; set; }
        public DateTime PublishedDate { get; set; }

        public string datePublished => PublishedDate.ToShortDateString();
        

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
