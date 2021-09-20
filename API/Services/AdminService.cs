﻿using API.Interfaces;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(UserManager<AppUser> userManager, ILogger<AdminService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ServiceResponseModel<List<UserWithRolesModel>>> GetUsersWithRoles(string requestor)
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
                serviceResponse.Message = $"Successfully listed User-Role relationships for user [{requestor}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to get list of User-Role relationship for user [{requestor}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
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
                    _logger.LogError(serviceResponse.Message);
                    return serviceResponse;
                }

                IList<string> userRoles = await _userManager.GetRolesAsync(user);
                IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

                if (result.Succeeded == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to add user [{username}] to roles [{selectedRoles.Except(userRoles)}]";
                    _logger.LogError(serviceResponse.Message);
                    return serviceResponse;
                }

                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

                if (result.Succeeded == false)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to remove user [{username}] from roles [{userRoles.Except(selectedRoles)}]";
                    _logger.LogError(serviceResponse.Message);
                    return serviceResponse;
                }

                serviceResponse.Success = true;
                serviceResponse.Data = await _userManager.GetRolesAsync(user);
                serviceResponse.Message = $"Successfully editted roles for user [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to add user [{username}] to roles [{roles}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
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
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Failed to get photos for moderation";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }
    }
}