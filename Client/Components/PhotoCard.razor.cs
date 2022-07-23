using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Client.Components;

public partial class PhotoCard
{
    [Parameter] public PhotoModel Photo { get; set; }

    [Parameter] public EventCallback<string> OnImageChanged { get; set; }

    private string _photoFilename { get; set; } = "./assets/user.png";

    protected override async Task OnParametersSetAsync()
    {
        _photoFilename = await MemberService.GetPhotoAsync(MemberStateService.AppUser.Username, Photo.Filename);
    }

    private async Task HandleSetMainPhotoAsync()
    {
        if (Photo.IsMain)
        {
            return;
        }

        ServiceResponseModel<string> result = await MemberService.SetMainPhotoAsync(MemberStateService.AppUser.Username, Photo.Id);

        if (result.Success)
        {
            Snackbar.Add("Main photo updated successfully", Severity.Success);
            await OnImageChanged.InvokeAsync();
        }
        else
        {
            Snackbar.Add($"{result.Message}", Severity.Error);
        }
    }

    private async Task HandleDeletePhotoAsync()
    {
        ServiceResponseModel<string> result = await MemberService.DeletePhotoAsync(MemberStateService.AppUser.Username, Photo);

        if (result.Success)
        {
            Snackbar.Add("Photo deleted successfully", Severity.Success);
            await OnImageChanged.InvokeAsync();
        }
        else
        {
            Snackbar.Add($"{result.Message}", Severity.Error);
        }
    }
}
