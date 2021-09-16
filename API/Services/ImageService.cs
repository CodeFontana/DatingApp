using API.Interfaces;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _appEnv;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IWebHostEnvironment appEnv,
                            IUserRepository userRepository,
                            ILogger<ImageService> logger)
        {
            _appEnv = appEnv;
            _userRepository = userRepository;
            _logger = logger;
        }

        private async Task<byte[]> GetUserImage(string username, string filename)
        {
            try
            {
                _ = username ?? throw new ArgumentException("Invalid username");
                _ = filename ?? throw new ArgumentException("Invalid filename");

                string imageFile = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{username}\{filename}");

                if (File.Exists(imageFile) == false)
                {
                    throw new FileNotFoundException($"Image not found [{imageFile ?? "null"}]");
                }

                return await File.ReadAllBytesAsync(imageFile);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get requested image [{filename ?? "null"}] for user [{username ?? "null"}]");

                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<ServiceResponseModel<byte[]>> GetImage(string username, string filename)
        {
            ServiceResponseModel<byte[]> serviceResponse = new();

            try
            {
                byte[] imageBytes = await GetUserImage(username, filename);

                serviceResponse.Success = true;
                serviceResponse.Data = imageBytes;
                serviceResponse.Message = $"Successfully loaded image for [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to get requested image [{filename ?? "null"}] for user [{username ?? "null"}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }

        public async Task<ServiceResponseModel<List<PhotoDownloadModel>>> GetImages(string username)
        {
            ServiceResponseModel<List<PhotoDownloadModel>> serviceResponse = new();
            List<PhotoDownloadModel> output = new();

            try
            {
                _ = username ?? throw new ArgumentException("Invalid username");
                MemberModel member = await _userRepository.GetMemberAsync(username);
                _ = member ?? throw new Exception($"Member with username [{username}] does not exist");
                
                string userImageDir = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{username}");

                if (Directory.Exists(userImageDir) == false)
                {
                    throw new DirectoryNotFoundException($"No images available for user {username}");
                }

                List<string> userImages = Directory.GetFiles(userImageDir)?.ToList();

                if (userImages.Count == 0)
                {
                    throw new FileNotFoundException($"No images available for user {username}");
                }

                foreach (string file in userImages)
                {
                    PhotoDownloadModel p = new()
                    {
                        Filename = file[(file.LastIndexOf('\\') + 1)..],
                        Data = await File.ReadAllBytesAsync(file)
                    };

                    output.Add(p);
                }

                serviceResponse.Success = true;
                serviceResponse.Data = output;
                serviceResponse.Message = $"Successfully loaded images for [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to get requested images for [{username ?? "null"}]";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }
    }
}
