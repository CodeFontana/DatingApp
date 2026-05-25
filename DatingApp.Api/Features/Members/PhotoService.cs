using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DatingApp.DataAccess.Entities;
using DatingApp.DataAccess.Interfaces;
using DatingApp.Contracts.Common;
using DatingApp.Contracts.Members.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DatingApp.Api.Features.Members;

public class PhotoService : IPhotoService
{
    private readonly ILogger<PhotoService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _appEnv;

    public PhotoService(ILogger<PhotoService> logger,
                        IUnitOfWork unitOfWork,
                        IWebHostEnvironment appEnv)
    {
        _appEnv = appEnv;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PhotoResponse>> AddPhotoAsync(string username, IEnumerable<IFormFile> files)
    {
        _logger.LogInformation($"Add photo for {username}...");
        ApiResponse<PhotoResponse> serviceResponse = new();
        long maxFileSize = 1024 * 1024 * 5;

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);

            AppUser? appUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username);
            IFormFile? file = files.FirstOrDefault();

            if (appUser is null)
            {
                throw new ArgumentException($"Invalid username [{username}]");
            }
            else if (file is null || file.Length == 0)
            {
                throw new ArgumentNullException($"Photo file is empty [{username}]");
            }
            else if (file.Length > maxFileSize)
            {
                throw new BadImageFormatException($"{file.FileName} of size {file.Length} bytes is larger than the limit of {maxFileSize} bytes [{username}]");
            }
            else if (IsValidImageFile(file) == false)
            {
                throw new BadImageFormatException($"{file.FileName} is not a supported image [{username}]");
            }
            else if (appUser.Photos.Count >= 8)
            {
                throw new Exception($"Photo storage limit reached [{username}]");
            }
            else if (file.Length > 0)
            {
                // Resize the image to 500x500
                using MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                //Image resizedFile = ResizeImage(Image.FromStream(memoryStream), new RectangleF(0, 0, 500, 500));

                // Build wwwroot/MemberData save path and filename
                string trustedName = Guid.NewGuid().ToString() + ".jpg";
                string uploadPath = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{appUser.UserName}");
                string fileName = Path.Combine(uploadPath, trustedName);

                Directory.CreateDirectory(uploadPath);
                using (Image image = Image.FromStream(memoryStream))
                {
                    image.Save(fileName);
                }

                Photo newPhoto = new()
                {
                    Filename = trustedName,
                    IsMain = false,
                };

                if (appUser.Photos.Count == 0)
                {
                    newPhoto.IsMain = true;
                }

                appUser.Photos.Add(newPhoto);

                if (await _unitOfWork.CompleteAsync())
                {
                    serviceResponse.Success = true;
                    serviceResponse.Data = MemberMapper.ToPhotoResponse(newPhoto);
                    serviceResponse.Message = $"Successfully added photo for user [{username}]";
                    _logger.LogInformation(serviceResponse.Message);
                }
                else
                {
                    throw new Exception($"Error adding photo to database [{username}]");
                }
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ApiResponse<byte[]>> GetPhotoAsync(string username, string filename)
    {
        _logger.LogInformation($"Get photo for {username}...");
        ApiResponse<byte[]> serviceResponse = new();

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
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ApiResponse<string>> SetMainPhotoAsync(string username, int photoId)
    {
        _logger.LogInformation($"Set main photo for {username}...");
        ApiResponse<string> serviceResponse = new();

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);

            AppUser appUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username)
                ?? throw new ArgumentException($"User not found [{username}]");
            Photo? photo = appUser.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo is null)
            {
                throw new ArgumentException($"Photo not found in database [{username}]");
            }

            if (photo.IsMain)
            {
                throw new ArgumentException($"This is already your main photo [{username}]");
            }

            foreach (Photo userPhoto in appUser.Photos)
            {
                userPhoto.IsMain = false;
            }

            photo.IsMain = true;

            if (await _unitOfWork.CompleteAsync())
            {
                serviceResponse.Success = true;
                serviceResponse.Message = $"Successfully set main photo for user [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Error saving main photo to database [{username}]");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ApiResponse<string>> DeletePhotoAsync(string username, PhotoResponse photo)
    {
        _logger.LogInformation($"Delete photo for {username}...");
        ApiResponse<string> serviceResponse = new();

        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentNullException.ThrowIfNull(photo);

            AppUser appUser = await _unitOfWork.MemberRepository.GetMemberByUsernameAsync(username)
                ?? throw new ArgumentException($"User not found [{username}]");
            Photo? existingPhoto = appUser.Photos.FirstOrDefault(x => x.Id == photo.Id)
                ?? throw new ArgumentException($"Photo not found in database [{username}]");

            appUser.Photos.Remove(existingPhoto);

            string path = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{appUser.UserName}");
            string fileName = Path.Combine(path, photo.Filename[(photo.Filename.LastIndexOf('/') + 1)..]);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            if (existingPhoto.IsMain && appUser.Photos.Count > 0)
            {
                appUser.Photos.First().IsMain = true;
            }

            if (await _unitOfWork.CompleteAsync())
            {
                serviceResponse.Success = true;
                serviceResponse.Message = $"Successfully delete photo for user [{username}]";
                _logger.LogInformation(serviceResponse.Message);
            }
            else
            {
                throw new Exception($"Failed to delete photo for user [{username}]: Error saving to database");
            }
        }
        catch (Exception e)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    private static bool IsValidImageFile(IFormFile file)
    {
        try
        {
            using Image _ = Image.FromStream(file.OpenReadStream());
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Ref: https://stackoverflow.com/questions/39068941/images-are-rotated-in-picturebox
    private static Image CorrectExifOrientation(Image image)
    {
        if (image == null)
        {
            throw new ArgumentException("Invalid image");
        }

        // Exif tag -- https://exiftool.org/TagNames/EXIF.html
        int orientationId = 0x0112;

        if (image.PropertyIdList.Contains(orientationId))
        {
            byte[]? orientationBytes = image.GetPropertyItem(orientationId)?.Value;
            if (orientationBytes is null || orientationBytes.Length == 0)
            {
                return image;
            }

            int orientation = orientationBytes[0];
            RotateFlipType rotateFlip;

            rotateFlip = orientation switch
            {
                1 => RotateFlipType.RotateNoneFlipNone,
                2 => RotateFlipType.RotateNoneFlipX,
                3 => RotateFlipType.Rotate180FlipNone,
                4 => RotateFlipType.Rotate180FlipX,
                5 => RotateFlipType.Rotate90FlipX,
                6 => RotateFlipType.Rotate90FlipNone,
                7 => RotateFlipType.Rotate270FlipX,
                8 => RotateFlipType.Rotate270FlipNone,
                _ => RotateFlipType.RotateNoneFlipNone,
            };

            if (rotateFlip != RotateFlipType.RotateNoneFlipNone)
            {
                image.RotateFlip(rotateFlip);
                image.RemovePropertyItem(orientationId);
            }
        }

        return image;
    }

    // Ref: https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
    public static Image ResizeImage(Image sourceImage, RectangleF destBounds)
    {
        // Use Exif tag to correct photo orientation
        sourceImage = CorrectExifOrientation(sourceImage);

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

