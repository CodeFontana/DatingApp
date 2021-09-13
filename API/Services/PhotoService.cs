using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PhotoService> _logger;

        public PhotoService(IUserRepository userRepository,
                            IMapper mapper,
                            ILogger<PhotoService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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
                    throw new ArgumentNullException($"Failed to add photo for user [{username}]: Empty file");
                }
                else if (file.Length > maxFileSize)
                {
                    throw new BadImageFormatException($"Failed to add photo for user [{username}]: {file.FileName} of size [{file.Length} bytes] is larger than the limit of [{maxFileSize} bytes]");
                }
                else if (IsValidImageFile(file) == false)
                {
                    throw new BadImageFormatException($"Failed to add photo for user [{username}]: {file.FileName} is not a supported image");
                }
                else if (appUser.Photos.Count >= 8)
                {
                    throw new Exception($"Failed to add photo for user [{username}]: Photo storage limit reached [8 photos]");
                }
                else if (file.Length > 0)
                {
                    // TODO: Use wwwroot path with upload folder for storing/hosting images to client. Can't use local files.

                    string trustedName = Guid.NewGuid().ToString() + ".jpg";
                    string basePath = Path.GetTempPath();
                    string uploadPath = Path.Combine(basePath, $@"DatingApp\uploads\members\{appUser.UserName}");
                    string fileName = Path.Combine(uploadPath, trustedName);
                    string fileUrl = "file://" + fileName.Replace("\\", "/");
                    
                    Directory.CreateDirectory(uploadPath);
                    using FileStream stream = File.Create(fileName);
                    await file.CopyToAsync(stream);

                    Photo newPhoto = new()
                    {
                        Url = fileUrl,
                        IsMain = false,
                    };

                    if (appUser.Photos.Count == 0)
                    {
                        newPhoto.IsMain = true;
                    }

                    appUser.Photos.Add(newPhoto);

                    if (await _userRepository.SaveAllAsync())
                    {
                        serviceResponse.Success = true;
                        serviceResponse.Data = _mapper.Map<PhotoModel>(newPhoto);
                        serviceResponse.Message = $"Successfully added photo for user [{username}]";
                        _logger.LogInformation(serviceResponse.Message);
                    }
                    else
                    {
                        serviceResponse.Success = false;
                        serviceResponse.Message = $"Failed to add photo for user [{username}]: Error saving to database";
                        _logger.LogInformation(serviceResponse.Message);
                    }
                }
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
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

        private static bool IsValidImageFile(IFormFile file)
        {
            try
            {
                Image isValidImage = Image.FromStream(file.OpenReadStream());
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
