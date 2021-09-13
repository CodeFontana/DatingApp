using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
                // https://localhost:5001/api/images
                string apiEndpoint = _config["apiLocation"].ToLower() + _config["imageEndpoint"].ToLower();

                // https://localhost:5001/api/images/brian/xyz.jpg --> /brian/xyz.jpg
                string requestItems = imageUrl.ToLower().Replace(apiEndpoint, "");

                HttpResponseMessage response = await _httpClient.GetAsync($"{apiEndpoint}{requestItems}");
                ServiceResponseModel<byte[]> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<byte[]>>(_options);

                if (result.Success)
                {
                    string username = requestItems.Substring(1, requestItems.LastIndexOf("/") - 1);
                    string filename = requestItems.Substring(requestItems.LastIndexOf("/") + 1);
                    string downloadFile = Path.Combine(Directory.GetCurrentDirectory(), $@"MemberData/{username}/{filename}");
                    // Save to wwwroot\MemberData\{username}\image.jpg
                    // Return new local URL
                    return "./assets/user.png"; // downloadFile
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
