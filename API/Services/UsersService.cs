using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
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

        public async Task<PaginationResponseModel<PaginationList<MemberModel>>> GetUsers(string requestor, UserParameters userParameters)
        {
            PaginationResponseModel<PaginationList<MemberModel>> pagedResponse = new();

            try
            {
                PaginationList<MemberModel> data = await _userRepository.GetMembersAsync(userParameters);

                pagedResponse.Success = true;
                pagedResponse.Data = data;
                pagedResponse.MetaData = data.MetaData;
                pagedResponse.Message = $"Successfully listed users for [{requestor}]";
                _logger.LogInformation(pagedResponse.Message);
            }
            catch (Exception e)
            {
                pagedResponse.Success = false;
                pagedResponse.Message = $"Failed to list users for [{requestor}]";
                _logger.LogError(pagedResponse.Message);
                _logger.LogError(e.Message);
            }

            return pagedResponse;
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
