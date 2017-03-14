using Blog.Turnmeup.Contexts;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.DL.Repositories;
using Blog.Turnmeup.DL.Services;
using Blog.Turnmeup.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Turnmeup.API.Infrastructure
{
    internal static class InstallerDl
    {

        public static void ConfigureDALServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>();
        
            services.AddTransient<IBaseRepository<Course>, BaseRepository<Course>>();
            services.AddTransient<IBaseService<Course>, BaseService<Course>>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IErrorHandler, ErrorHandler>();
        }

    }
}

