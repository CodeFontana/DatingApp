using DataAccessLibrary.Models;

namespace Client.Services;

public class MessageService : IMessageService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IPhotoService _photoService;
    private readonly IMemberStateService _memberStateService;
    private readonly JsonSerializerOptions _options;

    public MessageService(IConfiguration config,
                          HttpClient httpClient,
                          IPhotoService photoService,
                          IMemberStateService memberStateService)
    {
        _config = config;
        _httpClient = httpClient;
        _photoService = photoService;
        _memberStateService = memberStateService;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<ServiceResponseModel<MessageModel>> CreateMessageAsync(MessageCreateModel messageCreateModel)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, messageCreateModel);
        ServiceResponseModel<MessageModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<MessageModel>>(_options);
        return result;
    }

    public async Task<PaginationResponseModel<IEnumerable<MessageModel>>> GetMessagesForMemberAsync(MessageParameters messageParameters)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"];

        Dictionary<string, string> queryStringParam = new()
        {
            [nameof(messageParameters.PageNumber)] = messageParameters.PageNumber.ToString(),
            [nameof(messageParameters.PageSize)] = messageParameters.PageSize.ToString(),
            [nameof(messageParameters.Container)] = messageParameters.Container
        };

        using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
        PaginationResponseModel<IEnumerable<MessageModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<IEnumerable<MessageModel>>>(_options);

        if (response.Headers != null && response.Headers.Contains("Pagination"))
        {
            result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
        }

        if (result.Success)
        {
            foreach (MessageModel msg in result.Data)
            {
                if (msg.RecipientUsername == _memberStateService.AppUser.Username)
                {
                    msg.RecipientPhotoUrl = _memberStateService.MainPhoto;
                }
                else
                {
                    msg.RecipientPhotoUrl = await _photoService.GetPhotoAsync(msg.RecipientUsername, msg.RecipientPhotoUrl);
                }

                if (msg.SenderUsername == _memberStateService.AppUser.Username)
                {
                    msg.SenderPhotoUrl = _memberStateService.MainPhoto;
                }
                else
                {
                    msg.SenderPhotoUrl = await _photoService.GetPhotoAsync(msg.SenderUsername, msg.SenderPhotoUrl);
                }
            }
        }

        return result;
    }

    public async Task<ServiceResponseModel<IEnumerable<MessageModel>>> GetMessageThreadAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/thread/{username}"; ;
        using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
        ServiceResponseModel<IEnumerable<MessageModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<IEnumerable<MessageModel>>>(_options);

        if (result.Success)
        {
            foreach (MessageModel msg in result.Data)
            {
                if (msg.RecipientUsername == _memberStateService.AppUser.Username)
                {
                    msg.RecipientPhotoUrl = _memberStateService.MainPhoto;
                }
                else
                {
                    msg.RecipientPhotoUrl = await _photoService.GetPhotoAsync(msg.RecipientUsername, msg.RecipientPhotoUrl);
                }

                if (msg.SenderUsername == _memberStateService.AppUser.Username)
                {
                    msg.SenderPhotoUrl = _memberStateService.MainPhoto;
                }
                else
                {
                    msg.SenderPhotoUrl = await _photoService.GetPhotoAsync(msg.SenderUsername, msg.SenderPhotoUrl);
                }
            }
        }

        return result;
    }

    public async Task<ServiceResponseModel<string>> DeleteMessageAsync(int id)
    {
        string apiEndpoint = _config["apiLocation"] + _config["messagesEndpoint"] + $"/{id}";
        using HttpResponseMessage response = await _httpClient.DeleteAsync(apiEndpoint);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);
        return result;
    }
}
