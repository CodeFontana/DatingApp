using System.ComponentModel.DataAnnotations;

namespace DatingApp.Contracts.Messages.Requests;

public class CreateMessageRequest
{
    [MaxLength(50)]
    public string RecipientUsername { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
