using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Interfaces
{
    public interface IMemberService
    {
        Task<Tuple<bool, string, MemberModel>> GetMemberAsync(string username);
        Task<Tuple<bool, string, List<MemberModel>>> GetMembersAsync();
        Task<Tuple<bool, string>> UpdateMemberAsync(MemberUpdateModel memberUpdate);
    }
}