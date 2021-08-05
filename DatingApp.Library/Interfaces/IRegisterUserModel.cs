namespace DatingApp.Library.Interfaces
{
    public interface IRegisterUserModel
    {
        string Password { get; set; }
        string Username { get; set; }
        string ConfirmPassword { get; set; }
    }
}