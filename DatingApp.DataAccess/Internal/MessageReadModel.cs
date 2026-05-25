using System.Linq.Expressions;
using DatingApp.DataAccess.Entities;

namespace DatingApp.DataAccess.Internal;

public sealed class MessageReadModel
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

    public static readonly Expression<Func<Message, MessageReadModel>> Projection = m => new MessageReadModel
    {
        Id = m.Id,
        SenderId = m.SenderId,
        SenderUsername = m.SenderUsername,
        SenderPhotoUrl = m.Sender.Photos.Where(p => p.IsMain).Select(p => p.Filename).FirstOrDefault() ?? string.Empty,
        RecipientId = m.RecipientId,
        RecipientUsername = m.RecipientUsername,
        RecipientPhotoUrl = m.Recipient.Photos.Where(p => p.IsMain).Select(p => p.Filename).FirstOrDefault() ?? string.Empty,
        Content = m.Content,
        DateRead = m.DateRead,
        MessageSent = m.MessageSent,
        SenderDeleted = m.SenderDeleted,
        RecipientDeleted = m.RecipientDeleted
    };

    public static MessageReadModel FromEntity(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return new MessageReadModel
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderUsername = message.SenderUsername,
            SenderPhotoUrl = message.Sender?.Photos?.FirstOrDefault(p => p.IsMain)?.Filename ?? string.Empty,
            RecipientId = message.RecipientId,
            RecipientUsername = message.RecipientUsername,
            RecipientPhotoUrl = message.Recipient?.Photos?.FirstOrDefault(p => p.IsMain)?.Filename ?? string.Empty,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            SenderDeleted = message.SenderDeleted,
            RecipientDeleted = message.RecipientDeleted
        };
    }
}
