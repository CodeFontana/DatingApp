namespace DatingApp.DataAccess.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _db;

    public LikesRepository(DataContext context)
    {
        _db = context;
    }

    public async Task<UserLike?> GetUserLikeAsync(int sourceUserId, int likedUserId)
    {
        return await _db.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<PaginationList<MemberReadModel>> GetUserLikesAsync(LikesListCriteria criteria)
    {
        IQueryable<AppUser> users = _db.Users.OrderBy(u => u.UserName).AsQueryable();
        IQueryable<UserLike> likes = _db.Likes.AsQueryable();

        if (criteria.Predicate.ToLower().Equals("liked"))
        {
            likes = likes.Where(like => like.SourceUserId == criteria.UserId);
            users = likes.Select(like => like.LikedUser);
        }

        if (criteria.Predicate.ToLower().Equals("likedby"))
        {
            likes = likes.Where(like => like.LikedUserId == criteria.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        return await PaginationList<MemberReadModel>.CreateAsync(
            users.Select(MemberReadModel.Projection).AsNoTracking(),
            criteria.PageNumber,
            criteria.PageSize);
    }

    public async Task<AppUser?> GetUserWithLikesAsync(int userId)
    {
        return await _db.Users
            .Include(x => x.LikedUsers)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
