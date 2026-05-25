namespace DatingApp.Contracts.Members.Responses;

public class PhotoResponse
{
    public int Id { get; set; }
    public string Filename { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
