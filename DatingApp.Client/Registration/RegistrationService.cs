using DatingApp.Client.Authentication;
using DatingApp.Client.Interfaces;
using DatingApp.Library.Interfaces;
using DatingApp.Library.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Client.Registration
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
