namespace DataAccessLibrary.Interfaces
{
    public interface ILoginUserModel
    {
        string Password { get; set; }
        string Username { get; set; }
    }
}