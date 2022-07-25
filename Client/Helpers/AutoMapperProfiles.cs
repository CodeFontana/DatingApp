namespace Client.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<MemberModel, MemberUpdateModel>();
        CreateMap<MemberUpdateModel, MemberModel>();
    }
}
