namespace DataAccessLibrary.Interfaces
{
    public interface IRegisterUser
    {
        string Password { get; set; }
        string Username { get; set; }
        string ConfirmPassword { get; set; }
    }
}