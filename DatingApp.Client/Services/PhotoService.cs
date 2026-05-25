using DatingApp.Client.Http;

namespace DatingApp.Client.Services;

public class PhotoService : IPhotoService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IMemberService _memberService;
    private readonly JsonSerializerOptions _options;

    public PhotoService(IConfiguration config,
                        HttpClient httpClient,
                        IMemberService memberService)
    {
        _config = config;
        _httpClient = httpClient;
        _memberService = memberService;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<ApiResponse<PhotoResponse>> AddPhotoAsync(string username, MultipartFormDataContent content)
    {
        string apiEndpoint = _config["apiLocation"] + _config["addPhotoEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);
        ApiResponse<PhotoResponse> result = await response.Content.ReadApiResponseAsync<PhotoResponse>(_options);

        if (result.Success && result.Data is not null)
        {
            MemberResponse? member = (await _memberService.GetMemberAsync(username)).Data;
            if (member is not null)
            {
                member.Photos.Add(result.Data);

                if (result.Data.IsMain)
                {
                    member.MainPhotoFilename = await GetPhotoAsync(username, result.Data.Filename);
                }
            }
        }

        return result;
    }

    public async Task<string> GetPhotoAsync(string username, string filename)
    {
        if (string.IsNullOrWhiteSpace(filename) || filename.ToLower().EndsWith("user.png"))
        {
            return "./assets/user.png";
        }
        else if (filename.ToLower().StartsWith("http") ||
            filename.ToLower().StartsWith("data:image"))
        {
            return filename;
        }
        else
        {
            string apiEndpoint = _config["apiLocation"] + _config["getPhotoEndpoint"] + $"/{username}/{filename}";
            using HttpResponseMessage response = await _httpClient.GetAsync($"{apiEndpoint}");
            ApiResponse<byte[]> result = await response.Content.ReadApiResponseAsync<byte[]>(_options);

            if (result.Success && result.Data is not null)
            {
                string imageBase64 = Convert.ToBase64String(result.Data);
                return string.Format("data:image/jpg;base64,{0}", imageBase64);
            }
            else
            {
                return "./assets/user.png";
            }
        }
    }

    public async Task<ApiResponse<string>> SetMainPhotoAsync(string username, int photoId)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["setMainPhotoEndpoint"];
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, photoId);
        ApiResponse<string> result = await response.Content.ReadApiResponseAsync<string>(_options);

        if (result.Success)
        {
            MemberResponse? member = (await _memberService.GetMemberAsync(username)).Data;
            PhotoResponse? mainPhoto = member?.Photos.FirstOrDefault(x => x.Id == photoId);
            if (member is not null && mainPhoto is not null)
            {
                member.Photos.ToList().ForEach(x => x.IsMain = false);
                mainPhoto.IsMain = true;
                member.MainPhotoFilename = mainPhoto.Filename;
            }
        }

        return result;
    }

    public async Task<ApiResponse<string>> DeletePhotoAsync(string username, PhotoResponse photo)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException(nameof(username), "Invalid username");
        }

        ArgumentNullException.ThrowIfNull(photo);

        string apiEndpoint = _config["apiLocation"] + _config["deletePhotoEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, photo);
        ApiResponse<string> result = await response.Content.ReadApiResponseAsync<string>(_options);

        if (result.Success)
        {
            MemberResponse? member = (await _memberService.GetMemberAsync(username)).Data;
            PhotoResponse? p = member?.Photos.FirstOrDefault(x => x.Id == photo.Id);
            if (member is not null && p is not null)
            {
                member.Photos.Remove(p);

                if (photo.IsMain)
                {
                    if (member.Photos.Count > 0)
                    {
                        member.Photos[0].IsMain = true;
                        member.MainPhotoFilename = member.Photos[0].Filename;
                    }
                    else
                    {
                        member.MainPhotoFilename = string.Empty;
                    }
                }
            }
        }

        return result;
    }
}
