using AutoMapper;
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
        private readonly IMapper _mapper;

        public MemberService(IConfiguration config, HttpClient httpClient, IMapper mapper)
        {
            _config = config;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        private List<MemberModel> Members { get; set; } = new();

        public async Task<Tuple<bool, string, List<MemberModel>>> GetMembersAsync()
        {
            if (Members.Count > 0)
            {
                return new Tuple<bool, string, List<MemberModel>>(true, "OK", Members);
            }
            
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            HttpResponseMessage response = await _httpClient.GetAsync(apiEndpoint);

            if (response.IsSuccessStatusCode)
            {
                List<MemberModel> result = await response.Content.ReadFromJsonAsync<List<MemberModel>>();
                Members = result;
                return new Tuple<bool, string, List<MemberModel>>(true, "OK", result);
            }
            else
            {
                return new Tuple<bool, string, List<MemberModel>>(false, response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), null);
            }
        }

        public async Task<Tuple<bool, string, MemberModel>> GetMemberAsync(string username)
        {
            MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(username.ToLower()));

            if (member != null)
            {
                return new Tuple<bool, string, MemberModel>(true, "OK", member);
            }
            else
            {
                Members = new();
            }

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

        public async Task<Tuple<bool, string>> UpdateMemberAsync(MemberUpdateModel memberUpdate)
        {
            string apiEndpoint = _config["apiLocation"] + _config["usersEndpoint"];
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync(apiEndpoint, memberUpdate);

            if (response.IsSuccessStatusCode)
            {
                MemberModel member = Members.FirstOrDefault(x => x.Username.ToLower().Equals(memberUpdate.Username.ToLower()));
                _mapper.Map(memberUpdate, member);
                return new Tuple<bool, string>(true, "OK");
            }
            else
            {
                return new Tuple<bool, string>(false, response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
        }
    }
}
