using DatingApp.Client.Features.Members;

namespace DatingApp.Client.Features.Members.Pages;

public partial class MemberEdit
{
    [Inject] public required IMemberService MemberService { get; set; }
    [Inject] public required IMemberStateService MemberStateService { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }

    private bool _showError = false;
    private string _errorText = string.Empty;
    private bool _changesMade = false;
    private MemberUpdateRequest _memberUpdate = new();

    private MemberResponse? Profile => MemberStateService.Member;

    protected override async Task OnInitializedAsync()
    {
        await LoadMemberAsync();
    }

    private async Task LoadMemberAsync()
    {
        bool result = await MemberStateService.ReloadAppUserAsync();

        if (result && MemberStateService.Member is not null)
        {
            _memberUpdate = MemberUpdateRequestExtensions.FromMemberResponse(MemberStateService.Member);
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
        ApiResponse<string> result = await MemberService.UpdateMemberAsync(_memberUpdate);

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
