﻿using AutoMapper;
using Client.Interfaces;
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
        private readonly JsonSerializerOptions _options;
        private readonly IMapper _mapper;

        public MemberService(IConfiguration config, HttpClient httpClient, IMapper mapper)
        {
            _config = config;
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _mapper = mapper;
        }

        private List<MemberModel> Members { get; set; } = new();

        public async Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetMembersAsync()
        {
            if (Members.Count > 0)
            {
                return new ServiceResponseModel<IEnumerable<MemberModel>>()
                {
                    Success = true,
                    Data = Members,
                    Message = "Cached user list"
                };
            }

            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
            ServiceResponseModel<IEnumerable<MemberModel>> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<IEnumerable<MemberModel>>>(_options);

            if (result.Success)
            {
                Members = result.Data.ToList();
            }

            return result;
        }

        public async Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username)
        {
            MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));

            if (member != null)
            {
                return new ServiceResponseModel<MemberModel>()
                {
                    Success = true,
                    Data = member,
                    Message = "Cached user"
                };
            }
            else
            {
                Members = new();
            }

            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"] + $"/{username}";
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);
            return await response.Content.ReadFromJsonAsync<ServiceResponseModel<MemberModel>>(_options);
        }

        public async Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate)
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, memberUpdate);
            ServiceResponseModel<string> result = await response.Content.ReadFromJsonAsync<ServiceResponseModel<string>>(_options);

            if (result.Success)
            {
                MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(memberUpdate.Username.ToLower()));
                _mapper.Map(memberUpdate, member);
            }

            return result;
        }
    }
}
