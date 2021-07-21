using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.Client.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(IConfiguration config,
                                 HttpClient httpClient,
                                 ILocalStorageService localStorage)
        {
            _config = config;
            _httpClient = httpClient;
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsync<string>(_config["authTokenStorageKey"]);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            bool isAuthenticated = await NotifyUserAuthenticationAsync(token);

            if (isAuthenticated == false)
            {
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        JwtParser.ParseClaimsFromJwt(token), 
                        "jwtAuthType")));
        }

        public async Task<bool> NotifyUserAuthenticationAsync(string token)
        {
            bool isAuthenticated;
            Task<AuthenticationState> authState;

            try
            {
                ClaimsPrincipal authenticatedUser = new(
                    new ClaimsIdentity(
                        JwtParser.ParseClaimsFromJwt(token),
                        "jwtAuthType"));

                authState = Task.FromResult(new AuthenticationState(authenticatedUser));

                string authTokenStorageKey = _config["authTokenStorageKey"];
                await _localStorage.SetItemAsync(authTokenStorageKey, token);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                NotifyAuthenticationStateChanged(authState);
                isAuthenticated = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await NotifyUserLogoutAsync();
                isAuthenticated = false;
            }

            return isAuthenticated;
        }

        public async Task NotifyUserLogoutAsync()
        {
            string authTokenStorageKey = _config["authTokenStorageKey"];
            await _localStorage.RemoveItemAsync(authTokenStorageKey);
            Task<AuthenticationState> authState = Task.FromResult(_anonymous);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
