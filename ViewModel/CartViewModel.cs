using System;
using BookStore.Models;

namespace BookStore.ViewModel
{
    public class CartViewModel
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public DateTime DateBooked { get; set; }
        
    }
}