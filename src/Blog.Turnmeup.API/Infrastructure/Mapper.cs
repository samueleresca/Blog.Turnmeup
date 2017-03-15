using AutoMapper;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.API.Infrastructure
{
    public class MappingProfile : Profile
    {
        public  MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Course, CourseResponseModel>();
            CreateMap<CourseResponseModel, Course>();

            CreateMap<UserResponseModel, AppUser>();
            CreateMap<AppUser, UserResponseModel>();

            CreateMap<UserManager<UserResponseModel>, UserManager<AppUser>>();
            CreateMap<UserManager<AppUser>, UserManager<UserResponseModel>>();
        }
    }
}
