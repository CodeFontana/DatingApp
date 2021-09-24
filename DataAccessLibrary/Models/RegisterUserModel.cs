using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Please enter a username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(4, ErrorMessage="Password must be at least (4) characters")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}
