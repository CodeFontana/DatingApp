﻿namespace Client.Services;

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

    public async Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, MultipartFormDataContent content)
    {
        string apiEndpoint = _config["apiLocation"] + _config["addPhotoEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);
        ServiceResponseModel<PhotoModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<PhotoModel>>(_options);

        if (result.Success)
        {
            MemberModel member = (await _memberService.GetMemberAsync(username)).Data;
            member.Photos.Add(result.Data);

            if (result.Data.IsMain)
            {
                // Download image from URL
                // --> The API stores images securely, requiring a JWT bearer
                //     token to view/download the member's images. Because of
                //     this, we cannot just place the image URL inside an img
                //     tag. Instead, the image must be requested from an
                //     HttpClient that contains the proper JWT bearer token,
                //     which is what is happening here.
                member.MainPhotoFilename = await GetPhotoAsync(username, result.Data.Filename);
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
            ServiceResponseModel<byte[]> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<byte[]>>(_options);

            if (result.Success)
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

    public async Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException("Invalid username");
        }

        string apiEndpoint = _config["apiLocation"] + _config["setMainPhotoEndpoint"];
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, photoId);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

        if (result.Success)
        {
            MemberModel member = (await _memberService.GetMemberAsync(username)).Data;
            PhotoModel mainPhoto = member.Photos.FirstOrDefault(x => x.Id == photoId);
            member.Photos.ToList().ForEach(x => x.IsMain = false);
            mainPhoto.IsMain = true;
            member.MainPhotoFilename = mainPhoto.Filename;
        }

        return result;
    }

    public async Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, PhotoModel photo)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentNullException("Invalid username");
        }

        _ = photo ?? throw new ArgumentNullException("Invalid photo");

        string apiEndpoint = _config["apiLocation"] + _config["deletePhotoEndpoint"];
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, photo);
        ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

        if (result.Success)
        {
            MemberModel member = (await _memberService.GetMemberAsync(username)).Data;
            PhotoModel p = member.Photos.FirstOrDefault(x => x.Id == photo.Id);
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
                    member.MainPhotoFilename = null;
                }
            }

        }

        return result;
    }
}
