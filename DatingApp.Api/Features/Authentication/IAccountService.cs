using DatingApp.Contracts.Authentication.Requests;
using DatingApp.Contracts.Authentication.Responses;
using DatingApp.Contracts.Common;

namespace DatingApp.Api.Features.Authentication;

public interface IAccountService
{
    Task<ApiResponse<AuthResponse>> RegisterAsync(string? requestor, RegisterRequest registerUser);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest loginUser);
    Task<ApiResponse<AccountResponse>> GetAccountAsync(string requestor, string username);
    Task<ApiResponse<List<AccountResponse>>> GetAccountsAsync(string requestor);
    Task<ApiResponse<bool>> UpdateAccountAsync(string requestor, AccountUpdateRequest updateAccount);
    Task<ApiResponse<bool>> DeleteAccountAsync(string requestor, string username);
}
