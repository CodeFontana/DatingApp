using DataAccessLibrary.Entities;
using DataAccessLibrary.Interfaces;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Paging;
using System;

namespace DataAccessLibrary.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberModel> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberModel>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberModel>> GetMembersAsync(UserParameters userParameters)
        {
            IQueryable<AppUser> query = _context.Users.AsQueryable();
            
            query = query.Where(u => u.UserName != userParameters.CurrentUsername);
            query = query.Where(u => u.Gender.ToLower() == userParameters.Gender.ToLower());

            DateTime minDob = DateTime.Today.AddYears(-userParameters.MaxAge - 1);
            DateTime maxDob = DateTime.Today.AddYears(-userParameters.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            return await PagedList<MemberModel>
                .CreateAsync(query
                    .ProjectTo<MemberModel>(_mapper.ConfigurationProvider)
                    .AsNoTracking(),
                userParameters.PageNumber,
                userParameters.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
