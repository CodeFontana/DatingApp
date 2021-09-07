using API.Interfaces;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRoles()
        {
            ServiceResponseModel<List<UserWithRolesModel>> serviceResponse = new();

            try
            {
                var users = await _userManager.Users
                    .Include(r => r.UserRoles)
                    .ThenInclude(r => r.Role)
                    .OrderBy(u => u.UserName)
                    .Select(u => new
                    {
                        u.Id,
                        Username = u.UserName,
                        Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                    })
                    .ToListAsync();

                serviceResponse.Data = new List<UserWithRolesModel>();

                foreach (var item in users)
                {
                    serviceResponse.Data.Add(new UserWithRolesModel
                    {
                        Id = item.Id,
                        Username = item.Username,
                        Roles = item.Roles
                    });
                }

                serviceResponse.Success = true;
                serviceResponse.Message = "Successfully listed User-Role relationships";
                Console.WriteLine("Successfully listed User-Role relationships");
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Failed to get list of User-Role relationship";
                Console.WriteLine(e.Message);
                Console.WriteLine("Failed to get list of User-Role relationship");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponseModel<IList<string>>> EditRoles(string username, string roles)
        {
            ServiceResponseModel<IList<string>> serviceResponse = new();

            try
            {
                string[] selectedRoles = roles.Split(",").ToArray();
                AppUser user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Could not find user [{username}].";
                    Console.WriteLine($"Could not find user [{username}].");
                    return serviceResponse;
                }

                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

                if (result.Succeeded == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to add user [{username}] to roles [{selectedRoles.Except(userRoles)}]";
                    Console.WriteLine($"Failed to add user [{username}] to roles [{selectedRoles.Except(userRoles)}]");
                    return serviceResponse;
                }

                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

                if (result.Succeeded == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to remove user [{username}] from roles [{userRoles.Except(selectedRoles)}]";
                    Console.WriteLine($"Failed to remove user [{username}] from roles [{userRoles.Except(selectedRoles)}]");
                    return serviceResponse;
                }

                serviceResponse.Success = true;
                serviceResponse.Data = await _userManager.GetRolesAsync(user);
                serviceResponse.Message = $"Successfully editted roles for user [{username}]";
                Console.WriteLine($"Successfully editted roles for user [{username}]");
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to add user [{username}] to roles [{roles}]";
                Console.WriteLine(e.Message);
                Console.WriteLine($"Failed to add user [{username}] to roles [{roles}]");
            }

            return serviceResponse;
        }

        public ServiceResponseModel<string> GetPhotosForModeration()
        {
            ServiceResponseModel<string> serviceResponse = new();

            try
            {
                serviceResponse.Success = true;
                serviceResponse.Data = "TODO: Admins or moderators can see this";
                serviceResponse.Message = "TODO: Admins or moderators can see this";

            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Failed to get photos for moderation";
                Console.WriteLine(e.Message);
                Console.WriteLine("Failed to get photos for moderation");
            }

            return serviceResponse;
        }
    }
}
