using AutoMapper;
using Client.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using Microsoft.AspNetCore.WebUtilities;
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

        //private List<MemberModel> Members { get; set; } = new();

        public async Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("Invalid username");
            }
            
            //MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));

            //if (member != null)
            //{
            //    return new ServiceResponseModel<MemberModel>()
            //    {
            //        Success = true,
            //        Data = member,
            //        Message = "Cached user"
            //    };
            //}
            //else
            //{
            //    Members = new();
            //}

            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"] + $"/{username}";
            using HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
            return await response.Content.ReadFromJsonAsync<ServiceResponseModel<MemberModel>>(_options);
        }

        public async Task<PaginationResponseModel<IEnumerable<MemberModel>>> GetMembersAsync(UserParameters userParameters)
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];

            var queryStringParam = new Dictionary<string, string>
            {
                [nameof(userParameters.PageNumber)] = userParameters.PageNumber.ToString(),
                [nameof(userParameters.PageSize)] = userParameters.PageSize.ToString(),
                [nameof(userParameters.MinAge)] = userParameters.MinAge.ToString(),
                [nameof(userParameters.MaxAge)] = userParameters.MaxAge.ToString(),
                [nameof(userParameters.Gender)] = userParameters.Gender,
                [nameof(userParameters.OrderBy)] = userParameters.OrderBy
            };

            using HttpResponseMessage response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(apiEndpoint, queryStringParam));
            PaginationResponseModel<IEnumerable<MemberModel>> result = await response.Content.ReadFromJsonAsync<PaginationResponseModel<IEnumerable<MemberModel>>>(_options);

            if (response.Headers != null && response.Headers.Contains("Pagination"))
            {
                result.MetaData = JsonSerializer.Deserialize<PaginationModel>(response.Headers.GetValues("Pagination").First(), _options);
            }
            
            //if (result.Success)
            //{
            //    Members = result.Data.ToList();
            //}

            return result;
        }

        public async Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate)
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, memberUpdate);
            ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

            //if (result.Success)
            //{
            //    MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(memberUpdate.Username.ToLower()));
            //    _mapper.Map(memberUpdate, member);
            //}

            return result;
        }

        public async Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, MultipartFormDataContent content)
        {
            string apiEndpoint = _config["apiLocation"] + _config["addPhotoEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsync(apiEndpoint, content);
            ServiceResponseModel<PhotoModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<PhotoModel>>(_options);

            //if (result.Success)
            //{
            //    MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
            //    member.Photos.Add(result.Data);

            //    if (result.Data.IsMain)
            //    {
            //        // Download image from URL
            //        // --> The API stores images securely, requiring a JWT bearer
            //        //     token to view/download the member's images. Because of
            //        //     this, we cannot just place the image URL inside an img
            //        //     tag. Instead, the image must be requested from an
            //        //     HttpClient that contains the proper JWT bearer token,
            //        //     which is what is happening here.
            //        member.MainPhotoFilename = await GetPhotoAsync(username, result.Data.Filename);
            //    }
            //}

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

            //if (result.Success)
            //{
            //    MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
            //    PhotoModel mainPhoto = member.Photos.FirstOrDefault(x => x.Id == photoId);
            //    member.Photos.ToList().ForEach(x => x.IsMain = false);
            //    mainPhoto.IsMain = true;
            //    member.MainPhotoFilename = mainPhoto.Filename;
            //}

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
            using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, photo);
            ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

            //if (result.Success)
            //{
            //    MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));
            //    PhotoModel p = member.Photos.FirstOrDefault(x => x.Id == photo.Id);
            //    member.Photos.Remove(p);

            //    if (photo.IsMain)
            //    {
            //        if (member.Photos.Count > 0)
            //        {
            //            member.Photos[0].IsMain = true;
            //            member.MainPhotoFilename = member.Photos[0].Filename;
            //        }
            //        else
            //        {
            //            member.MainPhotoFilename = null;
            //        }
            //    }
            //}

            return result;
        }
    }
}
