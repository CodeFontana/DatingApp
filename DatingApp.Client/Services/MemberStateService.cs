namespace DatingApp.Client.Services;

public class MemberStateService : IMemberStateService
{
    private readonly IMemberService _memberService;
    private readonly IPhotoService _photoService;
    private MemberResponse? _member;
    private string _mainPhoto = "./assets/user.png";

    public MemberStateService(IMemberService memberService, IPhotoService photoService)
    {
        _memberService = memberService;
        _photoService = photoService;
    }

    public MemberResponse? Member => _member;

    public string MainPhoto => _mainPhoto;

    public event Action? OnChange;

    public async Task<bool> ReloadAppUserAsync()
    {
        return await SetAppUserAsync(_member?.Username);
    }

    public async Task<bool> SetAppUserAsync(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _member = null;
            _mainPhoto = "./assets/user.png";
            NotifyStateChanged();
            return true;
        }

        ApiResponse<MemberResponse> result = await _memberService.GetMemberAsync(username);

        if (result.Success && result.Data is not null)
        {
            _member = result.Data;
            await SetMainPhotoAsync(_member.MainPhotoFilename);
            NotifyStateChanged();
            return true;
        }

        return false;
    }

    public async Task SetMainPhotoAsync(string filename)
    {
        if (_member is null)
        {
            return;
        }

        _mainPhoto = await _photoService.GetPhotoAsync(_member.Username, filename);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
