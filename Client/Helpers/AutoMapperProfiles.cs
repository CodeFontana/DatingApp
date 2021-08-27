using AutoMapper;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<MemberModel, MemberUpdateModel>();
            CreateMap<MemberUpdateModel, MemberModel>();
        }
    }
}
