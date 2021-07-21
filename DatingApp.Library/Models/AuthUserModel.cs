using DatingApp.Library.Interfaces;

namespace DatingApp.Library.Models
{
    public class AuthUserModel : IAuthUserModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}