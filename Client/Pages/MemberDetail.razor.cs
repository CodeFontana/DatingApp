using AutoMapper.Execution;
using Client.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Client.Pages;

public partial class MemberDetail
{
    [Inject] IMemberService MemberService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }
    [Parameter] public string Username { get; set; }

    private MemberModel _member;
    private string _photoFilename = "./assets/user.png";
    private bool _showError = false;
    private string _errorText;

    protected override async Task OnParametersSetAsync()
    {
        ServiceResponseModel<MemberModel> result = await MemberService.GetMemberAsync(Username);

        if (result.Success)
        {
            _member = result.Data;
            _photoFilename = await MemberService.GetPhotoAsync(_member.Username, _member.MainPhotoFilename);
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }

    private async Task HandleLikeToggleAsync()
    {
        ServiceResponseModel<string> result = await MemberService.ToggleLikeAsync(_member.Username);

        if (result.Success)
        {
            Snackbar.Add($"Liked {_member.Username}", Severity.Success);
        }
        else
        {
            Snackbar.Add(result.Message, Severity.Error);
        }
    }
}
