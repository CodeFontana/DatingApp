﻿namespace API.Interfaces;

public interface IMemberService
{
    Task<ServiceResponseModel<MemberModel>> GetMemberAsync(string username, string requestor);
    Task<PaginationResponseModel<PaginationList<MemberModel>>> GetMembersAsync(string requestor, UserParameters userParameters);
    Task<ServiceResponseModel<string>> UpdateMemberAsync(string username, MemberUpdateModel memberUpdate);
}