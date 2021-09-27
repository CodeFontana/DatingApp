﻿using DataAccessLibrary.Models;
using DataAccessLibrary.Pagination;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUsersService
    {
        Task<ServiceResponseModel<MemberModel>> GetUser(string username, string requestor);
        Task<PaginationResponseModel<PaginationList<MemberModel>>> GetUsers(string requestor, UserParameters userParameters);
        Task<ServiceResponseModel<string>> UpdateUser(string username, MemberUpdateModel memberUpdate);
    }
}