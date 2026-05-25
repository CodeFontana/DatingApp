using System.ComponentModel.DataAnnotations;

namespace DatingApp.Contracts.Admin.Responses;

public class UserWithRolesResponse
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}
