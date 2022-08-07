using DataAccessLibrary.Models;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly IMemberRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _appEnv;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(IMemberRepository userRepository,
                        IMapper mapper,
                        IWebHostEnvironment appEnv,
                        ILogger<PhotoService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _appEnv = appEnv;
        _logger = logger;
    }

    public async Task<ServiceResponseModel<PhotoModel>> AddPhotoAsync(string username, IEnumerable<IFormFile> files)
    {
        _logger.LogInformation($"Add photo for {username}...");
        ServiceResponseModel<PhotoModel> serviceResponse = new();
        long maxFileSize = 1024 * 1024 * 5;

        try
        {
            _ = username ?? throw new ArgumentException("Invalid username");
            AppUser appUser = await _userRepository.GetMemberByUsernameAsync(username);
            IFormFile file = files.FirstOrDefault();

            if (appUser == null)
            {
                throw new ArgumentNullException($"Invalid username");
            }
            else if (file == null || file.Length == 0)
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
                //resizedFile.Save(fileName);
                Image.FromStream(memoryStream).Save(fileName);

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

                if (await _userRepository.SaveAllAsync())
                {
                    serviceResponse.Success = true;
                    serviceResponse.Data = _mapper.Map<PhotoModel>(newPhoto);
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

    public async Task<ServiceResponseModel<byte[]>> GetPhotoAsync(string username, string filename)
    {
        _logger.LogInformation($"Get photo for {username}...");
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
            serviceResponse.Message = e.Message;
            _logger.LogError(e.Message);
        }

        return serviceResponse;
    }

    public async Task<ServiceResponseModel<string>> SetMainPhotoAsync(string username, int photoId)
    {
        _logger.LogInformation($"Set main photo for {username}...");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            _ = username ?? throw new ArgumentException("Invalid username");

            AppUser appUser = await _userRepository.GetMemberByUsernameAsync(username);
            Photo p = appUser.Photos.FirstOrDefault(x => x.Id == photoId);

            if (p is not null)
            {
                if (p.IsMain)
                {
                    throw new ArgumentException($"This is already your main photo [{username}]");
                }

                appUser.Photos.ToList().ForEach(x => x.IsMain = false);
                p.IsMain = true;
            }
            else
            {
                throw new ArgumentException($"Photo not found in database [{username}]");
            }

            if (await _userRepository.SaveAllAsync())
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

    public async Task<ServiceResponseModel<string>> DeletePhotoAsync(string username, PhotoModel photo)
    {
        _logger.LogInformation($"Delete photo for {username}...");
        ServiceResponseModel<string> serviceResponse = new();

        try
        {
            _ = username ?? throw new ArgumentException("Invalid username");
            _ = photo ?? throw new ArgumentException("Invalid photo for deletion");

            AppUser appUser = await _userRepository.GetMemberByUsernameAsync(username);
            Photo p = appUser.Photos.FirstOrDefault(x => x.Id == photo.Id);

            if (p is not null)
            {
                appUser.Photos.Remove(p);

                string path = Path.Combine(_appEnv.ContentRootPath, $@"MemberData\{appUser.UserName}");
                string fileName = Path.Combine(path, photo.Filename[(photo.Filename.LastIndexOf("/") + 1)..]);

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                if (p.IsMain && appUser.Photos.Count > 0)
                {
                    appUser.Photos.First().IsMain = true;
                }
            }
            else
            {
                throw new ArgumentException($"Photo not found in database [{username}]");
            }

            if (await _userRepository.SaveAllAsync())
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
            Image isValidImage = Image.FromStream(file.OpenReadStream());
        }
        catch
        {
            return false;
        }

        return true;
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
            int orientation = (int)image.GetPropertyItem(orientationId).Value[0];
            RotateFlipType rotateFlip = RotateFlipType.RotateNoneFlipNone;

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
