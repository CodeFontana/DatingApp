using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Paging;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersService> _logger;

        public UsersService(IUserRepository userRepository, IMapper mapper, ILogger<UsersService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponseModel<MemberModel>> GetUser(string username, string requestor)
        {
            ServiceResponseModel<MemberModel> serviceResponse = new();

            try
            {
                serviceResponse.Success = true;
                serviceResponse.Data = await _userRepository.GetMemberAsync(username);
                serviceResponse.Message = $"Successfully retrieved [{username}] for [{requestor}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to retrieve [{username}] for [{requestor}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }

        public async Task<PagingResponseModel<PagedList<MemberModel>>> GetUsers(string requestor, UserParameters userParameters)
        {
            PagingResponseModel<PagedList<MemberModel>> pagingResponse = new();

            try
            {
                AppUser appUser = await _userRepository.GetUserByUsernameAsync(requestor);
                userParameters.CurrentUsername = appUser.UserName;

                if (string.IsNullOrWhiteSpace(userParameters.Gender))
                {
                    userParameters.Gender = appUser.Gender.ToLower() == "male" ? "female" : "male";
                }

                PagedList<MemberModel> data = await _userRepository.GetMembersAsync(userParameters);

                pagingResponse.Success = true;
                pagingResponse.Data = data;
                // pagingResponse.MetaData = data.MetaData; (to follow convention, we will add paging metadata to the response headers, instead of the body content)
                pagingResponse.Message = $"Successfully listed users for [{requestor}]";
                _logger.LogInformation(pagingResponse.Message);
            }
            catch (Exception e)
            {
                pagingResponse.Success = false;
                pagingResponse.Message = $"Failed to list users for [{requestor}]";
                _logger.LogError(pagingResponse.Message);
                _logger.LogError(e.Message);
            }

            return pagingResponse;
        }

        public async Task<ServiceResponseModel<string>> UpdateUser(string username, MemberUpdateModel memberUpdate)
        {
            ServiceResponseModel<string> serviceResponse = new();

            try
            {
                AppUser appUser = await _userRepository.GetUserByUsernameAsync(username);
                _mapper.Map(memberUpdate, appUser);
                _userRepository.Update(appUser);

                if (await _userRepository.SaveAllAsync())
                {
                    serviceResponse.Success = true;
                    serviceResponse.Data = $"Successfully updated user [{username}]";
                    serviceResponse.Message = $"Successfully updated user [{username}]";
                    _logger.LogInformation(serviceResponse.Message);
                }
                else
                {
                    throw new Exception($"Failed to update user [{username}]");
                }
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }
    }
}
