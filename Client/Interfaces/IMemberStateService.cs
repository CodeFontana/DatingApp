namespace Client.Interfaces;

public interface IMemberStateService
{
    MemberModel AppUser { get; }
    string MainPhoto { get; }

    event Action OnChange;

    Task<bool> ReloadAppUserAsync();
    Task<bool> SetAppUserAsync(string username);
    Task SetMainPhotoAsync(string filename);
}