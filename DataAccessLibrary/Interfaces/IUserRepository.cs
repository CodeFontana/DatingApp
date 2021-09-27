using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<PaginationList<MemberModel>> GetMembersAsync(UserParameters userParameters);
        Task<MemberModel> GetMemberAsync(string username);
    }
}
