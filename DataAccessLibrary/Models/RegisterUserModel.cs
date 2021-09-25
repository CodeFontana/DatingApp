using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Please enter a username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your preferred name")]
        [MaxLength(50, ErrorMessage = "Your preferred name must be less than (50) characters")]
        public string KnownAs { get; set; }

        [Required(ErrorMessage = "Please specify your biology")]
        [MaxLength(25)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please enter your date of birth")]
        [ValidBirthday(ErrorMessage = "You must be at least 18 years old")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-20);

        [Required(ErrorMessage = "Please enter your town or city")]
        [MaxLength(100, ErrorMessage = "Your town name must be less than (100) characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter your state or province")]
        [MaxLength(100, ErrorMessage = "Your state name must be less than (100) characters")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(4, ErrorMessage="Password must be at least (4) characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DisplayName("Confirm Password")]
        [Compare(nameof(Password), ErrorMessage = "Password does not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ValidBirthdayAttribute : RangeAttribute
    {
        public ValidBirthdayAttribute()
          : base(typeof(DateTime),
                  DateTime.Now.AddYears(-120).ToShortDateString(),
                  DateTime.Now.AddYears(-18).ToShortDateString())
        { }
    }
}
