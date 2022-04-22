using AutoMapper;
using Domain.Identity;
using Register.Web.Models;

namespace Register.Web.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserViewModel>()
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.UserName))
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(dest => dest.Email))               
                 .ForMember(dest => dest.Photo, opt => opt.MapFrom(dest => "/images/" + dest.Photo));

            CreateMap<RegisterViewModel, AppUser>()
                .ForMember(dest => dest.Photo, opt =>opt.MapFrom(x=>x.Photo))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(x => x.Password))              

                ;
        }
    }
}
