using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Client.Components;

public partial class MemberCard
{
    [Inject] NavigationManager NavManager { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public MemberModel Member { get; set; }

    private string _photoFilename = "./assets/user.png";

    protected override async Task OnParametersSetAsync()
    {
        _photoFilename = await MemberService.GetPhotoAsync(Member.Username, Member.MainPhotoFilename);
        Member.MainPhotoFilename = _photoFilename;
    }

    private void HandleUserClick()
    {
        NavManager.NavigateTo($"/member/{Member.Username}");
    }

    private async Task HandleLikeToggleAsync()
    {
        ServiceResponseModel<string> result = await MemberService.ToggleLikeAsync(Member.Username);

        if (result.Success)
        {
            Snackbar.Add($"Liked {Member.Username}", Severity.Success);
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
        }
    }
}
