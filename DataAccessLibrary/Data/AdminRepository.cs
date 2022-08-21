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

    public async Task EditRolesAsync(UserWithRolesModel userWithRoles)
    {
        if (string.IsNullOrWhiteSpace(userWithRoles.Username))
        {
            throw new ArgumentException("Invalid user for role modification");
        }
        
        AppUser user = await _userManager.FindByNameAsync(userWithRoles.Username);

        if (user == null)
        {
            throw new ArgumentException($"Could not find user [{userWithRoles.Username}]");
        }

        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        IdentityResult result = await _userManager.AddToRolesAsync(user, userWithRoles.Roles.Except(userRoles));

        if (result.Succeeded == false)
        {
            throw new Exception($"Failed to add user [{userWithRoles.Username}] to roles [{userWithRoles.Roles.Except(userRoles)}]");
        }

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(userWithRoles.Roles));

        if (result.Succeeded == false)
        {
            throw new Exception($"Failed to remove user [{userWithRoles.Username}] from roles [{userRoles.Except(userWithRoles.Roles)}]");
        }

        return;
    }
}
