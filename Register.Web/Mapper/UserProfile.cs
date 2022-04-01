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
                 .ForMember(dest => dest.Password, opt => opt.MapFrom(dest => dest.Password))
                 .ForMember(dest => dest.ConfirmPassword, opt => opt.MapFrom(dest => dest.ConfirmPassword))
                 .ForMember(dest => dest.Photo, opt => opt.MapFrom(dest => "images/" + dest.Photo));
        }
    }
}
