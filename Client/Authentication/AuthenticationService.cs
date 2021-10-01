﻿using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
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
        private readonly IAppUserService _appUserService;

        public AuthenticationService(IConfiguration config,
                                     HttpClient httpClient,
                                     AuthenticationStateProvider authStateProvider,
                                     IAppUserService appUserService)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _appUserService = appUserService;
        }

        public async Task<ServiceResponseModel<AuthUserModel>> LoginAsync(LoginUserModel loginUser)
        {
            string apiEndpoint = _config["apiLocation"] + _config["loginEndpoint"];
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(apiEndpoint, loginUser);
            ServiceResponseModel<AuthUserModel> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<AuthUserModel>>(_options);

            if (result.Success)
            {
                await ((AuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync(result.Data.Token);
                await _appUserService.SetAppUserAsync(result.Data.Username);
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogoutAsync();
            await _appUserService.SetAppUserAsync(null);
        }
    }
}
