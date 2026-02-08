namespace DataAccessLibrary.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _db;

    public LikesRepository(DataContext context)
    {
        _db = context;
    }

    public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId)
    {
        return await _db.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<PaginationList<MemberModel>> GetUserLikesAsync(LikesParameters likesParameters)
    {
        IQueryable<AppUser> users = _db.Users.OrderBy(u => u.UserName).AsQueryable();
        IQueryable<UserLike> likes = _db.Likes.AsQueryable();

        if (likesParameters.Predicate.ToLower().Equals("liked"))
        {
            likes = likes.Where(like => like.SourceUserId == likesParameters.UserId);
            users = likes.Select(like => like.LikedUser);
        }

        if (likesParameters.Predicate.ToLower().Equals("likedby"))
        {
            likes = likes.Where(like => like.LikedUserId == likesParameters.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        return await PaginationList<MemberModel>.CreateAsync(
            users.Select(MemberModel.Projection)
            .AsNoTracking(),
            likesParameters.PageNumber,
            likesParameters.PageSize);
    }

    public async Task<AppUser> GetUserWithLikesAsync(int userId)
    {
        return await _db.Users
            .Include(x => x.LikedUsers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
