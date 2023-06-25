

using AutoMapper;
using UserAPI.Data;
using UserAPI.Helper;

namespace Application.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<RequesDto, User>()
                .ForMember(d => d.Id, o => o.MapFrom(s => Encryption.GenerateEmailId(s.Email)));

            CreateMap<User, UserDto>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.MarketingConsent ? s.Email : null));

        }


    }


}