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
    public class ImageService : IImageService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public ImageService(IConfiguration config,
                            HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<string> RequestImageAsync(string imageUrl)
        {
            // Sample URL per API/Services/PhotoService.cs:
            // https://localhost:5001/api/images/brian/xyz.jpg

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
                // Images endpoint -- https://localhost:5001/api/images
                string apiEndpoint = _config["apiLocation"].ToLower() + _config["imageEndpoint"].ToLower();

                // Parse request fields -- https://localhost:5001/api/images/brian/xyz.jpg --> /brian/xyz.jpg
                string requestItems = imageUrl.ToLower().Replace(apiEndpoint, "");
                string username = requestItems[1..requestItems.LastIndexOf("/")];
                string filename = requestItems[(requestItems.LastIndexOf("/") + 1)..];

                HttpResponseMessage response = await _httpClient.GetAsync($"{apiEndpoint}{requestItems}");
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
