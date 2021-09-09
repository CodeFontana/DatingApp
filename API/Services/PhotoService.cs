using API.Interfaces;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PhotoService> _logger;

        public PhotoService(IUserRepository userRepository,
                            ILogger<PhotoService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, IEnumerable<IFormFile> files)
        {
            ServiceResponseModel<PhotoModel> serviceResponse = new();
            long maxFileSize = 1024 * 1024 * 5;

            try
            {
                AppUser appUser = await _userRepository.GetUserByUsernameAsync(username);
                IFormFile file = files.FirstOrDefault();

                if (file == null || file.Length == 0)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Failed to add photo for user [{username}]: Empty file";
                    _logger.LogError(serviceResponse.Message);
                }
                else if (file.Length > maxFileSize)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"failed to add photo for user [{username}]: {file.FileName} of size [{file.Length} bytes] is larger than the limit of [{maxFileSize} bytes]";
                    _logger.LogError(serviceResponse.Message);
                }
                else if (file.Length > 0)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), $@"uploads\members\{appUser.UserName}", file.FileName);
                    Directory.CreateDirectory(path);
                    await using FileStream fs = new(path, FileMode.Create);
                    await file.OpenReadStream().CopyToAsync(fs);
                    serviceResponse.Success = true;
                    serviceResponse.Message = $"Successfully added photo for user [{username}]";
                    _logger.LogInformation(serviceResponse.Message);
                }
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to add photo for user [{username}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, int photoId)
        {
            ServiceResponseModel<string> serviceResponse = new();

            try
            {
                AppUser appUser = await _userRepository.GetUserByUsernameAsync(username);

            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to delete photo for [{username}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }
    }
}
