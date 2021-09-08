using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetUsers()
        {
            ServiceResponseModel<IEnumerable<MemberModel>> serviceResponse = new();

            try
            {
                serviceResponse.Success = true;
                serviceResponse.Data = await _userRepository.GetMembersAsync();
                serviceResponse.Message = "Successfully listed users";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Failed to list users";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponseModel<MemberModel>> GetUser(string username)
        {
            ServiceResponseModel<MemberModel> serviceResponse = new();

            try
            {
                serviceResponse.Success = true;
                serviceResponse.Data = await _userRepository.GetMemberAsync(username);
                serviceResponse.Message = $"Successfully retrieved user [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to get user [{username}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
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
                    serviceResponse.Success = false;
                    serviceResponse.Data = $"Failed to update user [{username}]";
                    serviceResponse.Message = $"Failed to update user [{username}]";
                    _logger.LogError(serviceResponse.Message);
                }
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to update user [{memberUpdate.Username}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }
    }
}
