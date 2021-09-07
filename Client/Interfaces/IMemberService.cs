using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IMemberService
    {
        Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username);
        Task<ServiceResponseModel<IEnumerable<MemberModel>>> GetMembersAsync();
        Task<ServiceResponseModel<string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
    }
}