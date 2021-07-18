using Blazored.LocalStorage;
using DatingApp.Client.Authentication;
using DatingApp.Client.Interfaces;
using DatingApp.Library.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private string _authTokenStorageKey;

        public AuthenticationService(IConfiguration config,
                                     HttpClient httpClient,
                                     AuthenticationStateProvider authStateProvider,
                                     ILocalStorageService localStorage)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _authTokenStorageKey = config["authTokenStorageKey"];
        }

        public async Task<AuthUserModel> Login(LoginUserModel loginUser)
        {
            string apiEndpoint = _config["apiLocation"] + _config["loginEndpoint"];
            HttpResponseMessage authResult = await _httpClient.PostAsJsonAsync(apiEndpoint, loginUser);

            if (authResult.IsSuccessStatusCode)
            {
                string authContent = await authResult.Content.ReadAsStringAsync();
                AuthUserModel result = JsonSerializer.Deserialize<AuthUserModel>(authContent, _options);
                await _localStorage.SetItemAsync(_authTokenStorageKey, result.Token);
                ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
                return result;
            }
            else
            {
                Console.WriteLine($"Login failed: {authResult.ReasonPhrase}");
                return null;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(_authTokenStorageKey);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
