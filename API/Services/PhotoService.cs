using API.Interfaces;
using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
                    //using MemoryStream memoryStream = new();
                    //await file.CopyToAsync(memoryStream);
                    //Bitmap resizedFile = ResizeImage(Image.FromStream(memoryStream), 500, 500);

                    // Build wwwroot/MemberData save path and URL
                    string trustedName = Guid.NewGuid().ToString() + ".jpg";
                    string uploadPath = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{appUser.UserName}");
                    string fileName = Path.Combine(uploadPath, trustedName);

                    // URL for API access -- https://localhost:5001/api/images/brian/xyz.jpg
                    string apiUrl = $@"{requestUrl}api/Images/{appUser.UserName}/{trustedName}";

                    Directory.CreateDirectory(uploadPath);
                    //resizedFile.Save(fileName);
                    using FileStream stream = File.Create(fileName);
                    await file.CopyToAsync(stream);

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

        // https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
