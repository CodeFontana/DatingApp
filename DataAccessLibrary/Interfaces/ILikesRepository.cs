using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLikeAsync(int sourceUserId, int likedUserId);
        Task<IEnumerable<LikeUserModel>> GetUserLikesAsync(string predicate, int userId);
        Task<AppUser> GetUserWithLikesAsync(int userId);
    }
}