using AutoMapper;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Extensions;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberModel>()
                .ForMember(dest => dest.MainPhotoFilename, opt => opt.MapFrom(src =>
                    src.Photos.FirstOrDefault(x => x.IsMain).Filename))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoModel>();
            CreateMap<MemberUpdateModel, AppUser>();
        }
    }
}
