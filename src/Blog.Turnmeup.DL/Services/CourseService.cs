using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Blog.Turnmeup.Models;

namespace Blog.Turnmeup.DL.Services
{
    public class CourseService : IBaseService<Course>, ICourseService
    {
        private readonly IBaseService<Course> _courseService;

        public CourseService(IBaseService<Course> courseService)
        {
            _courseService = courseService;
        }


        public Task<IEnumerable<Course>> Get()
        {
            return _courseService.Get();
        }

        public Task<Course> GetById(int id)
        {
            return _courseService.GetById(id);
        }

        public IEnumerable<Course> Where(Expression<Func<Course, bool>> exp)
        {
            return _courseService.Where(exp);
        }

        public void AddOrUpdate(Course entry)
        {
            _courseService.AddOrUpdate(entry);
        }

        public void Remove(int id)
        {
            _courseService.Remove(id);
        }
    }
}
