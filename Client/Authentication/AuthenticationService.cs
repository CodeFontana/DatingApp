using Client.Interfaces;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
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

namespace Client.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthenticationService(IConfiguration config,
                                     HttpClient httpClient,
                                     AuthenticationStateProvider authStateProvider)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
        }

        public async Task<Tuple<AuthUserModel, string>> LoginAsync(LoginUserModel loginUser)
        {
            string apiEndpoint = _config["apiLocation"] + _config["loginEndpoint"];
            HttpResponseMessage authResult = await _httpClient.PostAsJsonAsync(apiEndpoint, loginUser);

            if (authResult.IsSuccessStatusCode)
            {
                string authContent = await authResult.Content.ReadAsStringAsync();
                AuthUserModel result = JsonSerializer.Deserialize<AuthUserModel>(authContent, _options);
                await ((AuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(result.Token);
                return new Tuple<AuthUserModel, string>(result, "Login successful");
            }
            else
            {
                return new Tuple<AuthUserModel, string>(null, authResult.ReasonPhrase);
            }
        }

        public async Task LogoutAsync()
        {
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogoutAsync();
        }
    }
}
