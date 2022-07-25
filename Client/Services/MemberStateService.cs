namespace Client.Services;

public class MemberStateService : IMemberStateService
{
    private readonly IMemberService _memberService;
    private readonly IPhotoService _photoService;
    private MemberModel _appUser;
    private string _mainPhoto;

    public MemberStateService(IMemberService memberService, IPhotoService photoService)
    {
        _mainPhoto = "./assets/user.png";
        _memberService = memberService;
        _photoService = photoService;
    }

    public MemberModel AppUser
    {
        get { return _appUser; }
    }

    public string MainPhoto
    {
        get { return _mainPhoto; }
    }

    public async Task<bool> ReloadAppUserAsync()
    {
        return await SetAppUserAsync(_appUser?.Username);
    }

    public async Task<bool> SetAppUserAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _appUser = null;
            _mainPhoto = "./assets/user.png";
            NotifyStateChanged();
            return true;
        }

        ServiceResponseModel<MemberModel> result = await _memberService.GetMemberAsync(username);

        if (result.Success)
        {
            _appUser = result.Data;
            await SetMainPhotoAsync(_appUser.MainPhotoFilename);
            NotifyStateChanged();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task SetMainPhotoAsync(string filename)
    {
        _mainPhoto = await _photoService.GetPhotoAsync(_appUser.Username, filename);
        NotifyStateChanged();
    }

    public event Action OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
