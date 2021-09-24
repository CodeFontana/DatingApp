using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Username required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }
}
