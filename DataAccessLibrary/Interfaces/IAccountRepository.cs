namespace DataAccessLibrary.Interfaces;

public interface IAccountRepository
{
    Task<AppUser> GetAccountAsync(string username);
    Task<List<AppUser>> GetAccountsAsync();
    Task<AppUser> CreateAccountAsync(RegisterUserModel registerUser);
    Task<AppUser> LoginAsync(LoginUserModel loginUser);
    Task UpdateAccountAsync(AccountUpdateModel updateAccount);
    Task<IdentityResult> DeleteAccountAsync(string requestor, string username);
}
