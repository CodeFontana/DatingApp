using API.Interfaces;
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
    public class ImagesService : IImagesService
    {
        private readonly IWebHostEnvironment _appEnv;
        private readonly ILogger<ImagesService> _logger;

        public ImagesService(IWebHostEnvironment appEnv, ILogger<ImagesService> logger)
        {
            _appEnv = appEnv;
            _logger = logger;
        }

        public async Task<ServiceResponseModel<byte[]>> GetImage(string username, string filename)
        {
            ServiceResponseModel<byte[]> serviceResponse = new();

            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new ArgumentException("Invalid username");
                }
                else if (string.IsNullOrWhiteSpace(filename))
                {
                    throw new ArgumentException("Invalid filename");
                }

                string imageFile = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{username}\{filename}");

                if (File.Exists(imageFile) == false)
                {
                    throw new FileNotFoundException("Image not found");
                }

                byte[] imageBytes = await File.ReadAllBytesAsync(imageFile);

                serviceResponse.Success = true;
                serviceResponse.Data = imageBytes;
                serviceResponse.Message = $"Successfully loaded image for [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Failed to get requested image";
                _logger.LogError(serviceResponse.Message);
                _logger.LogError(e.Message);
            }

            return serviceResponse;
        }
    }
}
