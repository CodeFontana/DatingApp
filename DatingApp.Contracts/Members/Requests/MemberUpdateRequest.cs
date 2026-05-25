using System.ComponentModel.DataAnnotations;

namespace DatingApp.Contracts.Members.Requests;

public class MemberUpdateRequest
{
    public string Username { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Introduction { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string LookingFor { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Interests { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string State { get; set; } = string.Empty;
}
