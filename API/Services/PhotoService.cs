using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _appEnv;
        private readonly ILogger<PhotoService> _logger;

        public PhotoService(IUserRepository userRepository,
                            IMapper mapper,
                            IWebHostEnvironment appEnv,
                            ILogger<PhotoService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _appEnv = appEnv;
            _logger = logger;
        }

        public async Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string requestUrl, string username, IEnumerable<IFormFile> files)
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
                    // Resize the image to 500x500
                    using MemoryStream memoryStream = new();
                    await file.CopyToAsync(memoryStream);
                    Image resizedFile = ResizeImage(Image.FromStream(memoryStream), new RectangleF(0, 0, 500, 500));

                    // Build wwwroot/MemberData save path and URL
                    string trustedName = Guid.NewGuid().ToString() + ".jpg";
                    string uploadPath = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{appUser.UserName}");
                    string fileName = Path.Combine(uploadPath, trustedName);

                    // URL for API access -- https://localhost:5001/api/image/brian/xyz.jpg
                    string apiUrl = $@"{requestUrl}api/Image/{appUser.UserName}/{trustedName}";

                    Directory.CreateDirectory(uploadPath);
                    resizedFile.Save(fileName);

                    Photo newPhoto = new()
                    {
                        Url = apiUrl,
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

        public async Task<ServiceResponseModel<byte[]>> GetPhotoAsync(string username, string filename)
        {
            ServiceResponseModel<byte[]> serviceResponse = new();

            try
            {
                _ = username ?? throw new ArgumentException("Invalid username");
                _ = filename ?? throw new ArgumentException("Invalid filename");

                string imageFile = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{username}\{filename}");

                if (File.Exists(imageFile) == false)
                {
                    throw new FileNotFoundException($"Image not found [{imageFile ?? "null"}]");
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
                serviceResponse.Message = $"Failed to get requested image [{filename ?? "null"}] for user [{username ?? "null"}]";
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

        // Ref: https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static Image ResizeImage(Image sourceImage, RectangleF destBounds)
        {
            RectangleF sourceBounds = new(0.0f, 0.0f, (float)sourceImage.Width, (float)sourceImage.Height);
            Image destinationImage = new Bitmap((int)destBounds.Width, (int)destBounds.Height);
            using Graphics g = Graphics.FromImage(destinationImage);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.FillRectangle(new SolidBrush(Color.Black), destBounds);

            float resizeRatio, sourceRatio;
            float scaleWidth, scaleHeight;

            sourceRatio = (float)sourceImage.Width / (float)sourceImage.Height;

            if (sourceRatio >= 1.0f)
            {
                // Landscape
                resizeRatio = destBounds.Width / sourceBounds.Width;
                scaleHeight = sourceBounds.Height * resizeRatio;
                float trimValue = destBounds.Height - scaleHeight;
                g.DrawImage(sourceImage, 0, (trimValue / 2), destBounds.Width, scaleHeight);
            }
            else
            {
                // Portrait
                resizeRatio = destBounds.Height / sourceBounds.Height;
                scaleWidth = sourceBounds.Width * resizeRatio;
                float trimValue = destBounds.Width - scaleWidth;
                g.DrawImage(sourceImage, (trimValue / 2), 0, scaleWidth, destBounds.Height);
            }

            return destinationImage;
        }
    }
}
