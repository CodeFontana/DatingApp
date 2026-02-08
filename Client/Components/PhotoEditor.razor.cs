namespace Client.Components;

public partial class PhotoEditor
{
    [Inject] IPhotoService PhotoService { get; set; }
    [Inject] IMemberStateService MemberStateService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public EventCallback<string> OnImageChanged { get; set; }

    private long _maxFileSize = 1024 * 1024 * 10;
    private bool _uploading = false;
    private bool _showError = false;
    private string _errorText;

    private static string _defaultDragClass = "relative rounded-lg border-2 border-dashed mt-5 mud-width-full mud-height-full d-flex justify-center align-center";
    private string _dragClass = _defaultDragClass;

    private void SetDragClass()
    {
        _dragClass = $"{_defaultDragClass} mud-border-primary";
    }

    private void ClearDragClass()
    {
        _dragClass = _defaultDragClass;
    }

    private async Task HandleImageUploadAsync(InputFileChangeEventArgs e)
    {
        try
        {
            ClearDragClass();
            _uploading = true;
            IBrowserFile imageFile = e.File;

            if (imageFile == null)
            {
                throw new Exception("Image file not found");
            }
            else if (imageFile.Size > _maxFileSize)
            {
                throw new Exception($"Image file exceeds maximum size [{_maxFileSize} bytes]");
            }

            StreamContent fileContent = new(imageFile.OpenReadStream(_maxFileSize));

            using MultipartFormDataContent content = new();
            content.Add(
                content: fileContent,
                name: "\"files\"",
                fileName: imageFile.Name);

            ServiceResponseModel<PhotoModel> result = await PhotoService.AddPhotoAsync(MemberStateService.Member.Username, content);

            if (result.Success)
            {
                Snackbar.Add("Photo added successfully", Severity.Success);
                _showError = false;
                await OnImageChanged.InvokeAsync();
            }
            else
            {
                _showError = true;
                _errorText = $"{result.Message}";
                Snackbar.Add($"{result.Message}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            _showError = true;
            _errorText = $"{ex.Message}";
            Snackbar.Add($"{ex.Message}", Severity.Error);
        }
        finally
        {
            _uploading = false;
        }
    }

    private async Task HandleImageChangedAsync()
    {
        await OnImageChanged.InvokeAsync();
    }
}
