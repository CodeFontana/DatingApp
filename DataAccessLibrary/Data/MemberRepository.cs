namespace DataAccessLibrary.Data;

public class MemberRepository : IMemberRepository
{
    private readonly DataContext _db;
    private readonly IMapper _mapper;

    public MemberRepository(DataContext context, IMapper mapper)
    {
        _db = context;
        _mapper = mapper;
    }

    public async Task<MemberModel> GetMemberAsync(string username)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(x => x.UserName == username)
            .ProjectTo<MemberModel>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<PaginationList<MemberModel>> GetMembersAsync(UserParameters userParameters)
    {
        IQueryable<AppUser> query = _db.Users.AsQueryable();

        query = query.Where(u => u.UserName != userParameters.CurrentUsername);
        query = query.Where(u => u.Gender.ToLower() == userParameters.Gender.ToLower());

        DateTime minDob = DateTime.Today.AddYears(-userParameters.MaxAge - 1);
        DateTime maxDob = DateTime.Today.AddYears(-userParameters.MinAge);

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParameters.OrderBy.ToLower() switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        return await PaginationList<MemberModel>
            .CreateAsync(
                query.ProjectTo<MemberModel>(_mapper.ConfigurationProvider)
                     .AsNoTracking(),
            userParameters.PageNumber,
            userParameters.PageSize);
    }

    public async Task<AppUser> GetMemberByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<AppUser> GetMemberByUsernameAsync(string username)
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

    public async Task<bool> SaveAllAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public void UpdateMember(AppUser user)
    {
        _db.Entry(user).State = EntityState.Modified;
    }
}
