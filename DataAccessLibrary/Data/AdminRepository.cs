namespace DataAccessLibrary.Data;

public class AdminRepository : IAdminRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public AdminRepository(UserManager<AppUser> userManager,
                           RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<string>> GetRolesAsync()
    {
        return await _roleManager.Roles
            .Select(x => x.Name)
            .OrderBy(x => x)
            .ToListAsync();
    }

    public async Task<List<UserWithRolesModel>> GetUsersWithRolesAsync()
    {
        return await _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .AsNoTracking()
            .Select(u => new UserWithRolesModel
            {
                Id = u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();
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
