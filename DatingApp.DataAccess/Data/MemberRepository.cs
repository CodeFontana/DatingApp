namespace DatingApp.DataAccess.Data;

public sealed class MemberRepository : IMemberRepository
{
    private readonly DataContext _db;

    public MemberRepository(DataContext context)
    {
        _db = context;
    }

    public async Task<MemberReadModel?> GetMemberAsync(string username)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(x => x.UserName == username)
            .Select(MemberReadModel.Projection)
            .SingleOrDefaultAsync();
    }

    public async Task<PaginationList<MemberReadModel>> GetMembersAsync(MemberListCriteria criteria)
    {
        IQueryable<AppUser> users = _db.Users.AsQueryable();

        users = users.Where(u => u.UserName != criteria.CurrentUsername);
        users = users.Where(u => u.Gender.ToLower() == criteria.Gender.ToLower());

        DateTime minDob = DateTime.Today.AddYears(-criteria.MaxAge - 1);
        DateTime maxDob = DateTime.Today.AddYears(-criteria.MinAge);

        users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        users = criteria.OrderBy.ToLower() switch
        {
            "created" => users.OrderByDescending(u => u.Created),
            _ => users.OrderByDescending(u => u.LastActive)
        };

        return await PaginationList<MemberReadModel>.CreateAsync(
            users.AsNoTracking().Select(MemberReadModel.Projection),
            criteria.PageNumber,
            criteria.PageSize);
    }

    public async Task<AppUser?> GetMemberByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetMemberByUsernameAsync(string username)
    {
        return await _db.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetMembersAsync()
    {
        return await _db.Users
            .AsNoTracking()
            .Include(p => p.Photos)
            .ToListAsync();
    }

    public void UpdateMember(AppUser user)
    {
        _db.Entry(user).State = EntityState.Modified;
    }
}
