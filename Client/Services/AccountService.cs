using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        public bool LoggedIn = false;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
        }

        public async Task Login(LoginModel loginUser)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Account/login", loginUser);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Login successful.");
                LoggedIn = true;
            }
            else
            {
                Console.WriteLine($"Login failed: {response.ReasonPhrase}");
                LoggedIn = false;
            }
        }

        public void Logout()
        {
            LoggedIn = false;
        }
    }
}
