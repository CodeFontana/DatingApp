namespace DataAccessLibrary.Models;

public class MemberUpdateModel
{
    public string Username { get; set; }

    [MaxLength(1000)]
    public string Introduction { get; set; }

    [MaxLength(1000)]
    public string LookingFor { get; set; }

    [MaxLength(1000)]
    public string Interests { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(100)]
    public string State { get; set; }
}
