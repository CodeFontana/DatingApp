namespace DataAccessLibrary.Entities;

[Table("Messages")]
public class Message
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    [MaxLength(50)]
    public string SenderUsername { get; set; } = string.Empty;

    [ForeignKey(nameof(SenderId))]
    [InverseProperty(nameof(AppUser.MessagesSent))]
    public AppUser Sender { get; set; } = null!;

    public int RecipientId { get; set; }

    [MaxLength(50)]
    public string RecipientUsername { get; set; } = string.Empty;

    [ForeignKey(nameof(RecipientId))]
    [InverseProperty(nameof(AppUser.MessagesReceived))]
    public AppUser Recipient { get; set; } = null!;

    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTime? DateRead { get; set; }

    public DateTime MessageSent { get; set; } = DateTime.UtcNow;

    public bool SenderDeleted { get; set; } = false;

    public bool RecipientDeleted { get; set; } = false;
}
