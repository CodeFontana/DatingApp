namespace Client.Services;

public class MemberStateService : IMemberStateService
{
    private readonly IMemberService _memberService;
    private readonly IPhotoService _photoService;

    public MemberStateService(IMemberService memberService, IPhotoService photoService)
    {
        _mainPhoto = "./assets/user.png";
        _memberService = memberService;
        _photoService = photoService;
    }

    private MemberModel _member;
    public MemberModel Member
    {
        get { return _member; }
    }

    private string _mainPhoto;
    public string MainPhoto
    {
        get { return _mainPhoto; }
    }

    public async Task<bool> ReloadAppUserAsync()
    {
        return await SetAppUserAsync(_member?.Username);
    }

    public async Task<bool> SetAppUserAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _member = null;
            _mainPhoto = "./assets/user.png";
            NotifyStateChanged();
            return true;
        }

        ServiceResponseModel<MemberModel> result = await _memberService.GetMemberAsync(username);

        if (result.Success)
        {
            _member = result.Data;
            await SetMainPhotoAsync(_member.MainPhotoFilename);
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
        _mainPhoto = await _photoService.GetPhotoAsync(_member.Username, filename);
        NotifyStateChanged();
    }

    public event Action OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
