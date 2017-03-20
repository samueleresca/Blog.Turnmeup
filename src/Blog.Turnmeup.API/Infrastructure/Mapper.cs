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

            CreateMap<UserModel, AppUser>();
            CreateMap<AppUser, UserModel>();

            CreateMap<UserManager<UserModel>, UserManager<AppUser>>();
            CreateMap<UserManager<AppUser>, UserManager<UserModel>>();
        }
    }
}
