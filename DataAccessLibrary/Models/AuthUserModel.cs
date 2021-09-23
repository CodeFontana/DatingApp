using DataAccessLibrary.Interfaces;

namespace DataAccessLibrary.Models
{
    public record AuthUserModel
    {
        public string Username { get; init; }
        public string Token { get; init; }
        public string PhotoFilename { get; init; }
    }
}