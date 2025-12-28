using System.ComponentModel.DataAnnotations;

namespace CloseFriendMyanamr.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        // [Phone(ErrorMessage = "Enter a valid phone number")]
        [RegularExpression(
        @"^(?:\+?95|0?9)\d{7,9}$",
        ErrorMessage = "Enter a valid Myanmar phone number"
    )]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
