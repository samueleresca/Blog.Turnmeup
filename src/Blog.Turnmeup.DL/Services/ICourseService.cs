using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Turnmeup.Models;

namespace Blog.Turnmeup.DL.Services
{
    public interface ICourseService
    {
        void AddOrUpdate(Course entry);
        Task<IEnumerable<Course>> Get();
        Task<Course> GetById(int id);
        void Remove(int id);
        IEnumerable<Course> Where(Expression<Func<Course, bool>> exp);
    }
}