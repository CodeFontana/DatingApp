namespace DataAccessLibrary.Models;

public class MessageModel
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    [MaxLength(50)]
    public string SenderUsername { get; set; }

    public string SenderPhotoUrl { get; set; }

    public int RecipientId { get; set; }

    [MaxLength(50)]
    public string RecipientUsername { get; set; }

    public string RecipientPhotoUrl { get; set; }

    [MaxLength(2000)]
    public string Content { get; set; }

    public DateTime? DateRead { get; set; }

    public DateTime MessageSent { get; set; }
}
