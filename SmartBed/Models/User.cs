using System.ComponentModel.DataAnnotations;

namespace SmartBed.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }


        // Full Name
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z ]+$",
            ErrorMessage = "Full name should contain letters and spaces only.")]
        public string FullName { get; set; }


        // Email
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }


        // Phone
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Phone number must contain exactly 10 digits.")]
        public string Phone { get; set; }


        // Username
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3,
            ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }


        // Password
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 6,
            ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string Password { get; set; }
    }
}
