namespace DataAccessLibrary.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UnitOfWork(DataContext db,
                      UserManager<AppUser> userManager,
                      RoleManager<AppRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public DataContext Db => _db;
    public UserManager<AppUser> UserManager => _userManager;
    public RoleManager<AppRole> RoleManager => _roleManager;
    public IAccountRepository AccountRepository => new AccountRepository(_db, _userManager);
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
