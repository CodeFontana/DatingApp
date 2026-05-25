namespace DatingApp.Contracts.Members.Responses;

public sealed class MemberResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string MainPhotoFilename { get; set; } = string.Empty;
    public int Age { get; set; }
    public string KnownAs { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Introduction { get; set; } = string.Empty;
    public string LookingFor { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public IList<PhotoResponse> Photos { get; set; } = [];
    public DateTime CacheTime { get; set; }
}
