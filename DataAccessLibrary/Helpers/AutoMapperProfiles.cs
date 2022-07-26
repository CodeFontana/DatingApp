namespace DataAccessLibrary.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberModel>()
            .ForMember(dest => dest.MainPhotoFilename, opt => opt.MapFrom(src =>
                src.Photos.FirstOrDefault(x => x.IsMain).Filename))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<RegisterUserModel, AppUser>();
        CreateMap<Photo, PhotoModel>();
        CreateMap<MemberUpdateModel, AppUser>();
        CreateMap<Message, MessageModel>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Filename))
            .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Filename));
    }
}
