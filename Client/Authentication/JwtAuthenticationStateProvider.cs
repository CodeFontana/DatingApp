using Microsoft.JSInterop;

namespace Client.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly IMemberStateService _memberStateService;
    private readonly IMemberService _memberService;
    private readonly IJSRuntime _jsRuntime;
    private readonly IPresenceService _presenceService;
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationState _anonymous;
    private Task _authStateMonitor;
    private CancellationTokenSource _authStateMonitoringTokenSource;
    private bool _isAuthenticated = false;

    public JwtAuthenticationStateProvider(IConfiguration config,
                                          HttpClient httpClient,
                                          IMemberStateService memberStateService,
                                          IMemberService memberService,
                                          IJSRuntime jsRuntime,
                                          IPresenceService presenceService,
                                          NavigationManager navigationManager)
    {
        _config = config;
        _httpClient = httpClient;
        _memberStateService = memberStateService;
        _memberService = memberService;
        _jsRuntime = jsRuntime;
        _presenceService = presenceService;
        _navigationManager = navigationManager;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            string localToken = LocalStorageValueCompat.FromBrowser(
                await _jsRuntime.InvokeAsync<string>("localStorage.getItem", _config["authTokenStorageKey"]));

            if (string.IsNullOrWhiteSpace(localToken))
            {
                return _anonymous;
            }

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.ReadToken(localToken);
            DateTime tokenExpiryDate = token.ValidTo;

            // If there is no valid 'exp' claim then 'ValidTo' returns DateTime.MinValue.
            if (tokenExpiryDate == DateTime.MinValue)
            {
                Console.WriteLine("Invalid JWT [Missing 'exp' claim]");
                return _anonymous;
            }

            // If the token is in the past then you can't use it.
            if (tokenExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine($"Invalid JWT [Token expired on {tokenExpiryDate.ToLocalTime()}]");
                return _anonymous;
            }

            bool isAuthenticated = await NotifyUserAuthenticationAsync(localToken);

            if (isAuthenticated == false)
            {
                return _anonymous;
            }

            return new AuthenticationState(
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        JwtParser.ParseClaimsFromJwt(localToken),
                        "jwtAuthType")));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return _anonymous;
        }
    }

    public async Task AuthenticationStateMonitor(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            if (_isAuthenticated == false)
            {
                await NotifyUserLogoutAsync();
                _navigationManager.NavigateTo("/sessionexpired", false);
                break;
            }

            await Task.Delay(5000);
        }
    }

    public async Task<bool> NotifyUserAuthenticationAsync(string token)
    {
        Task<AuthenticationState> authState;

        try
        {
            ClaimsPrincipal authenticatedUser = new(
                new ClaimsIdentity(
                    JwtParser.ParseClaimsFromJwt(token),
                    "jwtAuthType"));

            authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            await _memberStateService.SetAppUserAsync(authenticatedUser.Identity.Name);

            string authTokenStorageKey = _config["authTokenStorageKey"];
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", authTokenStorageKey, token);

            NotifyAuthenticationStateChanged(authState);
            _isAuthenticated = true;

            if (_authStateMonitor == null || _authStateMonitor.IsCompleted)
            {
                _authStateMonitoringTokenSource = new();
                _authStateMonitor = await Task.Factory.StartNew(() =>
                    AuthenticationStateMonitor(
                        _authStateMonitoringTokenSource.Token), 
                        TaskCreationOptions.LongRunning);
            }

            await _presenceService.ConnectAsync(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await NotifyUserLogoutAsync();
            _isAuthenticated = false;
        }

        return _isAuthenticated;
    }

    public async Task NotifyUserLogoutAsync()
    {
        await _presenceService.DisconnectAsync();
        _authStateMonitoringTokenSource?.Cancel();
        string authTokenStorageKey = _config["authTokenStorageKey"];
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", authTokenStorageKey);
        Task<AuthenticationState> authState = Task.FromResult(_anonymous);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _memberService.MemberCache.Clear();
        _memberService.MemberListCache.Clear();
        NotifyAuthenticationStateChanged(authState);
        await _memberStateService.SetAppUserAsync(null);
        _isAuthenticated = false;
    }
}
