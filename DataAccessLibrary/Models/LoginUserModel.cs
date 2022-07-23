namespace DataAccessLibrary.Models;

public class LoginUserModel
{
    [Required(ErrorMessage = "Please enter a username")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Please enter a password")]
    public string Password { get; set; }
}
