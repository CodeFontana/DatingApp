using DatingApp.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Library.Models
{
    public class RegisterUserModel : IRegisterUserModel
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
