using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModel
{
    public class RegisterViewModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime DoB { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        

    }
}