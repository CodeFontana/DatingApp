using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
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

        public UsersService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetUsers()
        {
            ServiceResponseModel<IEnumerable<MemberModel>> serviceResponse = new();

            try
            {
                serviceResponse.Success = true;
                serviceResponse.Data = await _userRepository.GetMembersAsync();
                serviceResponse.Message = "Successfully listed users";
                Console.WriteLine("Successfully listed users");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Failed to list users");
                serviceResponse.Success = false;
                serviceResponse.Message = "Failed to list users";
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
                Console.WriteLine($"Successfully retrieved user [{username}]");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Failed to get user [{username}]");
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to get user [{username}]";
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
                    Console.WriteLine($"Successfully updated user [{username}]");
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Data = $"Failed to update user [{username}]";
                    serviceResponse.Message = $"Failed to update user [{username}]";
                    Console.WriteLine($"Failed to update user [{username}]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Failed to update user [{memberUpdate.Username}]");
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to update user [{memberUpdate.Username}]";
            }

            return serviceResponse;
        }
    }
}
