namespace DataAccessLibrary.Models;

public class MessageCreateModel
{
    [MaxLength(50)]
    public string RecipientUsername { get; set; }

    [MaxLength(2000)]
    public string Content { get; set; }
}
