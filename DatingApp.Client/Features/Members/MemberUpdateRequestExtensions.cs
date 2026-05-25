using DatingApp.Contracts.Members.Requests;
using DatingApp.Contracts.Members.Responses;

namespace DatingApp.Client.Features.Members;

public static class MemberUpdateRequestExtensions
{
    public static MemberUpdateRequest FromMemberResponse(MemberResponse? member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return new MemberUpdateRequest
        {
            Username = member.Username,
            Introduction = member.Introduction,
            LookingFor = member.LookingFor,
            Interests = member.Interests,
            City = member.City,
            State = member.State
        };
    }
}
