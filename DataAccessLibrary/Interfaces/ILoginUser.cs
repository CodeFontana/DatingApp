namespace DataAccessLibrary.Interfaces
{
    public interface ILoginUser
    {
        string Password { get; set; }
        string Username { get; set; }
    }
}