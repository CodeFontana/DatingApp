using DatingApp.Contracts.Messages.Requests;
using DatingApp.Contracts.Messages.Responses;
using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Internal;
using DatingApp.DataAccess.Pagination;

namespace DatingApp.Api.Features.Messages;

internal static class MessageMapper
{
    public static MessageListCriteria ToCriteria(MessageListQuery query, string username) => new()
    {
        PageNumber = query.PageNumber,
        PageSize = query.PageSize,
        Username = username,
        Container = query.Container
    };

    public static MessageResponse ToResponse(MessageReadModel model) => new()
    {
        Id = model.Id,
        SenderId = model.SenderId,
        SenderUsername = model.SenderUsername,
        SenderPhotoUrl = model.SenderPhotoUrl,
        RecipientId = model.RecipientId,
        RecipientUsername = model.RecipientUsername,
        RecipientPhotoUrl = model.RecipientPhotoUrl,
        Content = model.Content,
        DateRead = model.DateRead,
        MessageSent = model.MessageSent,
        SenderDeleted = model.SenderDeleted,
        RecipientDeleted = model.RecipientDeleted
    };

    public static MessageResponse ToResponse(Message message) => MessageReadModel.FromEntity(message) is MessageReadModel read
        ? ToResponse(read)
        : new MessageResponse
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderUsername = message.SenderUsername,
            RecipientId = message.RecipientId,
            RecipientUsername = message.RecipientUsername,
            Content = message.Content,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            SenderDeleted = message.SenderDeleted,
            RecipientDeleted = message.RecipientDeleted
        };
}
