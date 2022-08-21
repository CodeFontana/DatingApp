namespace DataAccessLibrary.Data;

public class AdminRepository : IAdminRepository
{
    private readonly UserManager<AppUser> _userManager;

    public AdminRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PaginationList<UserWithRolesModel>> GetUsersWithRolesAsync(PaginationParameters pageParameters)
    {
        return await PaginationList<UserWithRolesModel>.CreateAsync(
            _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .AsNoTracking()
            .Select(u => new UserWithRolesModel
            {
                Id = u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }), 
            pageParameters.PageNumber,
            pageParameters.PageSize);
    }

    public async Task<IList<string>> EditRolesAsync(string username, string roles)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Invalid user for role modification");
        }
        
        string[] selectedRoles = roles.Split(",").ToArray();
        AppUser user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            throw new ArgumentException($"Could not find user [{username}]");
        }

        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        IdentityResult result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (result.Succeeded == false)
        {
            throw new Exception($"Failed to add user [{username}] to roles [{selectedRoles.Except(userRoles)}]");
        }

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (result.Succeeded == false)
        {
            throw new Exception($"Failed to remove user [{username}] from roles [{userRoles.Except(selectedRoles)}]");
        }

        return await _userManager.GetRolesAsync(user);
    }
}
