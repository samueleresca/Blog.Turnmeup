using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.Models;

namespace Blog.Turnmeup.DL.Services
{
    public interface ICourseService
    {
        void AddOrUpdate(CourseResponseModel entry);
        Task<IEnumerable<CourseResponseModel>> GetAsync();
        Task<CourseResponseModel> GetById(int id);
        void Remove(int id);
        IEnumerable<CourseResponseModel> Where(Expression<Func<Course, bool>> exp);
    }
}