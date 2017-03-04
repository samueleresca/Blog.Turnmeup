using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Turnmeup.Models;

namespace Blog.Turnmeup.DL.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        IEnumerable<T> Where(Expression<Func<T,bool>>  exp );
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
