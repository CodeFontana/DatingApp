namespace DataAccessLibrary.Entities;

[Table("Messages")]
public class Message
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    [MaxLength(50)]
    public string SenderUsername { get; set; }

    public AppUser Sender { get; set; }

    public int RecipientId { get; set; }

    [MaxLength(50)]
    public string RecipientUsername { get; set; }

    public AppUser Recipient { get; set; }

    [MaxLength(2000)]
    public string Content { get; set; }

    public DateTime? DateRead { get; set; }

    public DateTime MessageSent { get; set; } = DateTime.UtcNow;

    public bool SenderDeleted { get; set; } = false;

    public bool RecipientDeleted { get; set; } = false;
}
