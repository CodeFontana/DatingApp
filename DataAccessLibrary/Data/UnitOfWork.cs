namespace DataAccessLibrary.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UnitOfWork(DataContext db,
                      UserManager<AppUser> userManager,
                      RoleManager<AppRole> roleManager,
                      SignInManager<AppUser> signInManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public DataContext Db => _db;
    public UserManager<AppUser> UserManager => _userManager;
    public RoleManager<AppRole> RoleManager => _roleManager;
    public SignInManager<AppUser> SignInManager => _signInManager;
    public IAccountRepository AccountRepository => new AccountRepository(_db, _userManager, _signInManager);
    public IAdminRepository AdminRepository => new AdminRepository(_userManager, _roleManager);
    public IMemberRepository MemberRepository => new MemberRepository(_db);
    public ILikesRepository LikesRepository => new LikesRepository(_db);
    public IMessageRepository MessageRepository => new MessageRepository(_db);

    public async Task<bool> CompleteAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _db.ChangeTracker.HasChanges();
    }
}
