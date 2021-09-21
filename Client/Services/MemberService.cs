using AutoMapper;
using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class MemberService : IMemberService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IMapper _mapper;

        public MemberService(IConfiguration config,
                             HttpClient httpClient,
                             IMapper mapper)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _mapper = mapper;
        }

        private List<MemberModel> Members { get; set; } = new();

        public async Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetMembersAsync()
        {
            if (Members.Count > 0)
            {
                return new ServiceResponseModel<IEnumerable<MemberModel>>()
                {
                    Success = true,
                    Data = Members,
                    Message = "Cached user list"
                };
            }

            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
            ServiceResponseModel<IEnumerable<MemberModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<IEnumerable<MemberModel>>>(_options);

            if (result.Success)
            {
                Members = result.Data.ToList();
            }

            return result;
        }

        public async Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username)
        {
            MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));

            if (member != null)
            {
                return new ServiceResponseModel<MemberModel>()
                {
                    Success = true,
                    Data = member,
                    Message = "Cached user"
                };
            }
            else
            {
                Members = new();
            }

            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"] + $"/{username}";
            using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
            return await response.Content.ReadFromJsonAsync<ServiceResponseModel<MemberModel>>(_options);
        }

        public async Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate)
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, memberUpdate);
            ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

            if (result.Success)
            {
                MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(memberUpdate.Username.ToLower()));
                _mapper.Map(memberUpdate, member);
            }

            return result;
        }

        public async Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, MultipartFormDataContent content)
        {
            string apiEndpoint = _config["apiLocation"] + _config["addPhotoEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);
            ServiceResponseModel<PhotoModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<PhotoModel>>(_options);

            if (result.Success)
            {
                MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
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
                    member.PhotoUrl = await GetPhotoAsync(result.Data.Url);
                }
            }

            return result;
        }

        public async Task<string> GetPhotoAsync(string imageUrl)
        {
            // Sample URL per API/Services/PhotoService.cs:
            // https://localhost:5001/api/image/brian/xyz.jpg

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return "./assets/user.png";
            }
            else if (imageUrl.ToLower().Contains(_config["imageEndpoint"].ToLower()) == false)
            {
                return imageUrl;
            }

            bool validUrl = Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (validUrl == false)
            {
                return "./assets/user.png";
            }

            if (imageUrl.ToLower().StartsWith(_config["apiLocation"].ToLower()))
            {
                // Images endpoint -- https://localhost:5001/api/image
                string apiEndpoint = _config["apiLocation"].ToLower() + _config["imageEndpoint"].ToLower();

                // Parse request fields -- https://localhost:5001/api/image/brian/xyz.jpg --> /brian/xyz.jpg
                string requestItems = imageUrl.ToLower().Replace(apiEndpoint, "");
                string username = requestItems[1..requestItems.LastIndexOf("/")];
                string filename = requestItems[(requestItems.LastIndexOf("/") + 1)..];

                using HttpResponseMessage response = await _httpClient.GetAsync($"{apiEndpoint}{requestItems}");
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
            else
            {
                return imageUrl;
            }
        }
    }
}
