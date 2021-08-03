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
using System.Threading.Tasks;

namespace DatingApp.Client.Registration
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationService _authService;

        public RegistrationService(IConfiguration config,
                                   HttpClient httpClient,
                                   IAuthenticationService authService)
        {
            _config = config;
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<IAuthUserModel> RegisterAsync(IRegisterUserModel registerUser)
        {
            string apiEndpoint = _config["apiLocation"] + _config["registerEndpoint"];
            HttpResponseMessage regResult = await _httpClient.PostAsJsonAsync(apiEndpoint, registerUser);

            if (regResult.IsSuccessStatusCode)
            {
                LoginUserModel loginUser = new();
                loginUser.Username = registerUser.Username;
                loginUser.Password = registerUser.Password;
                return await _authService.LoginAsync(loginUser);
            }
            else
            {
                Console.WriteLine($"Register user failed: {regResult.ReasonPhrase}");
                return null;
            }
        }
    }
}
