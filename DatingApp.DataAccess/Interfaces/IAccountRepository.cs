namespace DatingApp.DataAccess.Interfaces;

public interface IAccountRepository
{
    Task<AppUser?> GetAccountAsync(string username);
    Task<List<AppUser>> GetAccountsAsync();
    Task<AppUser> CreateAccountAsync(RegisterAccountData registerUser);
    Task<AppUser> LoginAsync(LoginCredentials loginUser);
    Task UpdateAccountAsync(AccountUpdateData updateAccount);
    Task<IdentityResult> DeleteAccountAsync(string requestor, string username);
}
