using System.Linq.Expressions;

namespace DataAccessLibrary.Models;

public sealed class MessageModel
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

    public bool SenderDeleted { get; set; }

    public bool RecipientDeleted { get; set; }

    public static readonly Expression<Func<Message, MessageModel>> Projection = m => new MessageModel
    {
        Id = m.Id,
        SenderId = m.SenderId,
        SenderUsername = m.SenderUsername,
        SenderPhotoUrl = m.Sender.Photos.Where(p => p.IsMain).Select(p => p.Filename).FirstOrDefault(),
        RecipientId = m.RecipientId,
        RecipientUsername = m.RecipientUsername,
        RecipientPhotoUrl = m.Recipient.Photos.Where(p => p.IsMain).Select(p => p.Filename).FirstOrDefault(),
        Content = m.Content,
        DateRead = m.DateRead,
        MessageSent = m.MessageSent,
        SenderDeleted = m.SenderDeleted,
        RecipientDeleted = m.RecipientDeleted
    };

    public static MessageModel FromEntity(Message message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return new MessageModel
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderUsername = message.SenderUsername,
            SenderPhotoUrl = message.Sender?.Photos?.FirstOrDefault(p => p.IsMain)?.Filename,
            RecipientId = message.RecipientId,
            RecipientUsername = message.RecipientUsername,
            RecipientPhotoUrl = message.Recipient?.Photos?.FirstOrDefault(p => p.IsMain)?.Filename,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            SenderDeleted = message.SenderDeleted,
            RecipientDeleted = message.RecipientDeleted
        };
    }
}
