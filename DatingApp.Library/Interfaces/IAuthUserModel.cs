namespace DatingApp.Library.Interfaces
{
    public interface IAuthUserModel
    {
        string Token { get; set; }
        string Username { get; set; }
    }
}