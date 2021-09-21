using API.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _appEnv;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IWebHostEnvironment appEnv,
                            ILogger<ImageService> logger)
        {
            _appEnv = appEnv;
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
    }
}
