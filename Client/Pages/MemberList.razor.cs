using Client.Interfaces;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Pages;

public partial class MemberList
{
    private List<MemberModel> _members = new();
    private PaginationModel _metaData;
    private bool _showError = false;
    private string _errorText;

    [Inject] IAppUserService AppUserService { get; set; }
    [Inject] IMemberService MemberService { get; set; }
    [Inject] ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(MemberService.MembersFilter.Gender))
        {
            if (AppUserService.AppUser.Gender.ToLower().Equals("female"))
            {
                MemberService.MembersFilter.Gender = "male";
            }
            else
            {
                MemberService.MembersFilter.Gender = "female";
            }
        }

        await LoadMembersAsync();
    }

    private async Task LoadMembersAsync()
    {
        PaginationResponseModel<IEnumerable<MemberModel>> result = await MemberService.GetMembersAsync(MemberService.MembersFilter);

        if (result.Success)
        {
            _showError = false;
            _members = result.Data.ToList();
            _metaData = result.MetaData;
        }
        else
        {
            _showError = true;
            _errorText = $"Request failed: {result.Message}";
            Snackbar.Add($"Request failed: {result.Message}", Severity.Error);
        }
    }

    private async Task HandleFilterSubmitAsync()
    {
        if (MemberService.MembersFilter.MinAge > MemberService.MembersFilter.MaxAge)
        {
            _showError = true;
            _errorText = "Please validate age filters";
        }
        else
        {
            _showError = false;
            await LoadMembersAsync();
        }
    }

    private async Task HandleFilterResetAsync()
    {
        MemberService.MembersFilter.PageNumber = 1;
        MemberService.MembersFilter.PageSize = 10;
        MemberService.MembersFilter.MinAge = 18;
        MemberService.MembersFilter.MaxAge = 99;
        MemberService.MembersFilter.OrderBy = "LastActive";

        if (AppUserService.AppUser.Gender.ToLower().Equals("female"))
        {
            MemberService.MembersFilter.Gender = "male";
        }
        else
        {
            MemberService.MembersFilter.Gender = "female";
        }

        await LoadMembersAsync();
    }

    private async Task HandleSortSubmitAsync(string sortValue)
    {
        MemberService.MembersFilter.OrderBy = sortValue.ToLower();
        await HandleFilterSubmitAsync();
    }

    private async Task HandlePageChangedAsync(int page)
    {
        MemberService.MembersFilter.PageNumber = page;
        _members = null;
        await LoadMembersAsync();
    }
}
