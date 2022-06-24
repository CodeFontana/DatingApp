using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public RegistrationService(IConfiguration config,
                                   HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<ServiceResponseModel<AuthUserModel>> RegisterAsync(RegisterUserModel registerUser)
        {
            string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, registerUser);
            return await response.Content.ReadFromJsonAsync<ServiceResponseModel<AuthUserModel>>(_options);
        }
    }
}
