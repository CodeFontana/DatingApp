using System.ComponentModel.DataAnnotations;

namespace DatingApp.Contracts.Messages.Responses;

public sealed class MessageResponse
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
    public string SenderPhotoUrl { get; set; } = string.Empty;
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; } = string.Empty;
    public string RecipientPhotoUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
}
