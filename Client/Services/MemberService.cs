﻿using Client.Interfaces;
using DataAccessLibrary.Models;
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
    public class MemberService : IMemberService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public MemberService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<Tuple<bool, string, List<MemberModel>>> GetMembersAsync()
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                List<MemberModel> result = await response.Content.ReadFromJsonAsync<List<MemberModel>>();
                return new Tuple<bool, string, List<MemberModel>>(true, "OK", result);
            }
            else
            {
                return new Tuple<bool, string, List<MemberModel>>(false, response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), null);
            }
        }

        public async Task<Tuple<bool, string, MemberModel>> GetMemberAsync(string username)
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"] + $"/{username}";
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                MemberModel result = await response.Content.ReadFromJsonAsync<MemberModel>();
                return new Tuple<bool, string, MemberModel>(true, "OK", result);
            }
            else
            {
                return new Tuple<bool, string, MemberModel>(false, response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), null);
            }
        }
    }
}
