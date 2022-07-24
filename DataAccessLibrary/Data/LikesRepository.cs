namespace DataAccessLibrary.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<IEnumerable<LikeUserModel>> GetUserLikesAsync(string predicate, int userId)
    {
        IQueryable<AppUser> users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        IQueryable<UserLike> likes = _context.Likes.AsQueryable();

        if (predicate.ToLower().Equals("liked"))
        {
            likes = likes.Where(like => like.SourceUserId == userId);
            users = likes.Select(like => like.LikedUser);
        }

        if (predicate.ToLower().Equals("likedby"))
        {
            likes = likes.Where(like => like.LikedUserId == userId);
            users = likes.Select(like => like.SourceUser);
        }

        return await users.Select(user => new LikeUserModel
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            MainPhoto = user.Photos.FirstOrDefault(p => p.IsMain).Filename,
            City = user.City,
            Id = user.Id
        }).ToListAsync();
    }

    public async Task<AppUser> GetUserWithLikesAsync(int userId)
    {
        return await _context.Users
            .Include(x => x.LikedUsers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
