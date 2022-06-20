using AutoMapper;
using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Client.Pages;

public partial class MemberEdit
{
    [Inject] IMemberService MemberService { get; set; }
    [Inject] IAppUserService AppUserService { get; set; }
    [Inject] IMapper Mapper { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    private bool _showError = false;
    private string _errorText;
    private bool _changesMade = false;
    private MemberUpdateModel _memberUpdate = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadMemberAsync();
    }

    private async Task LoadMemberAsync()
    {
        bool result = await AppUserService.ReloadAppUserAsync();

        if (result)
        {
            Mapper.Map(AppUserService.AppUser, _memberUpdate);
        }
        else
        {
            _errorText = "Failed to load your profile";
            _showError = true;
        }
    }

    private void OnProfileUpdated(ChangeEventArgs e)
    {
        _changesMade = true;
    }

    private async Task HandleValidSubmitAsync()
    {
        ServiceResponseModel<string> result = await MemberService.UpdateMemberAsync(_memberUpdate);

        if (result.Success)
        {
            Snackbar.Add("Profile updated successfully", Severity.Success);
            _changesMade = false;
            _showError = false;
            await LoadMemberAsync();
        }
        else
        {
            _showError = true;
            _errorText = $"Profile update failed: {result.Message}";
            Snackbar.Add(_errorText, Severity.Error);
        }
    }

    private async Task ImageChangedCallbackAsync()
    {
        await LoadMemberAsync();
    }
}
