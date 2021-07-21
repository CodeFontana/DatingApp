using DatingApp.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Library.Models
{
    public class LoginUserModel : ILoginUserModel
    {
        [Required(ErrorMessage = "Username required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
