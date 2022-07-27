using DataAccessLibrary.Models;

namespace API.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[ServiceFilter(typeof(UserActivity))]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
	{
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResponseModel<PaginationList<MessageModel>>>> GetMessagesForMemberAsync([FromQuery] MessageParameters messageParameters)
    {
        messageParameters.Username = User.Identity.Name;
        PaginationResponseModel<PaginationList<MessageModel>> response = await _messageService.GetMessagesForMemberAsync(User.Identity.Name, messageParameters);

        if (response.Success)
        {
            Response.AddPaginationHeader(response.MetaData);
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<ServiceResponseModel<IEnumerable<MessageModel>>>> GetMessageThreadAsync(string username)
    {
        ServiceResponseModel<IEnumerable<MessageModel>> response = await _messageService.GetMessageThreadAsync(User.Identity.Name, username);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }


    [HttpPost]
	public async Task<ActionResult<ServiceResponseModel<MessageModel>>> CreateMessageAsync(MessageCreateModel messageCreateModel)
	{
        ServiceResponseModel<MessageModel> response = await _messageService.CreateMessageAsync(User.Identity.Name, messageCreateModel);

        if (response.Success)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest(response);
        }
    }
}
