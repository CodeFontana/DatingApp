using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Client.Authentication
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
            try
            {
                string localToken = await _localStorage.GetItemAsync<string>(_config["authTokenStorageKey"]);

                if (string.IsNullOrWhiteSpace(localToken))
                {
                    return _anonymous;
                }

                JwtSecurityTokenHandler tokenHandler = new();
                SecurityToken token = tokenHandler.ReadToken(localToken);
                var tokenExpiryDate = token.ValidTo;

                Console.WriteLine($"Token expires: {tokenExpiryDate.ToLocalTime()}");

                // If there is no valid 'exp' claim then 'ValidTo' returns DateTime.MinValue.
                if (tokenExpiryDate == DateTime.MinValue)
                {
                    Console.WriteLine("Invalid JWT [Missing 'exp' claim].");
                    return _anonymous;
                }

                // If the token is in the past then you can't use it.
                if (tokenExpiryDate < DateTime.UtcNow)
                {
                    Console.WriteLine($"Invalid JWT [Token expired on {tokenExpiryDate}].");
                    return _anonymous;
                }

                bool isAuthenticated = await NotifyUserAuthenticationAsync(localToken);

                if (isAuthenticated == false)
                {
                    return _anonymous;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", localToken);

                return new AuthenticationState(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            JwtParser.ParseClaimsFromJwt(localToken),
                            "jwtAuthType")));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return _anonymous;
            }
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
