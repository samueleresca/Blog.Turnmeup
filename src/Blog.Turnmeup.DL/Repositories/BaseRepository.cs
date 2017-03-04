using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Turnmeup.Contexts;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Turnmeup.DL.Repositories
{
    public  class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;

        private readonly DbSet<T> _entities;

        private readonly IErrorHandler _errorHandler;

        public BaseRepository(DataContext context, IErrorHandler errorHandler)
        {
            _context = context;
            _entities = context.Set<T>();
            _errorHandler = errorHandler;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }
        public async Task<T> GetById(int id)
        {
           return await _entities.SingleOrDefaultAsync(s => s.Id == id);
        }

        public IEnumerable<T> Where(Expression<Func<T,bool>> exp)
        {
            return  _entities.Where(exp);
        }
        public async void Insert(T entity)
        {
            if (entity == null) throw new ArgumentNullException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.EntityNull), "", "Input data is null"));
            await _entities.AddAsync(entity);
            _context.SaveChanges();
        }
        public async void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.EntityNull), "", "Input data is null"));

            var oldEntity = await _context.FindAsync<T>(entity.Id);
            _context.Entry(oldEntity).CurrentValues.SetValues(entity);
            _context.SaveChanges();
        }
        public void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.EntityNull), "", "Input data is null"));

            _entities.Remove(entity);
            _context.SaveChanges();
        }

    }
}
