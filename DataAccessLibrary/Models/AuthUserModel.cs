using DataAccessLibrary.Interfaces;

namespace DataAccessLibrary.Models
{
    public class AuthUserModel : IAuthUserModel
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}