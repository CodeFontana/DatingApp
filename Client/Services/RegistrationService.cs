using Client.Authentication;
using Client.Interfaces;
using DataAccessLibrary.Interfaces;
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
    public class RegistrationService : IRegistrationService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public RegistrationService(IConfiguration config,
                                   HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<Tuple<bool, string>> RegisterAsync(IRegisterUserModel registerUser)
        {
            string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"];
            HttpResponseMessage regResult = await _httpClient.PostAsJsonAsync(apiEndpoint, registerUser);

            if (regResult.IsSuccessStatusCode)
            {
                return new Tuple<bool, string>(true, "User created successfully.");
            }
            else
            {
                return new Tuple<bool, string>(false, regResult.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }
    }
}
