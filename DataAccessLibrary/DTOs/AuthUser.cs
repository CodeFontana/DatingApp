using DataAccessLibrary.Interfaces;

namespace DataAccessLibrary.Models
{
    public class AuthUser : IAuthUser
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}