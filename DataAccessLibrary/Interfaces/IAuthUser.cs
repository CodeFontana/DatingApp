namespace DataAccessLibrary.Interfaces
{
    public interface IAuthUser
    {
        string Token { get; set; }
        string Username { get; set; }
    }
}