using System.Linq;
using System.Threading.Tasks;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;

        public UsersService(IUsersRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<AppUser> Get()
        {
            return _repository.Get();
        }

        public AppUser GetByEmail(string email)
        {
            return _repository.GetByEmail(email);
        }

        public Task<IdentityResult> Create(AppUser user, string password)
        {
            return _repository.Create(user, password);
        }

        public async Task<IdentityResult> Delete(AppUser user)
        {
            return await _repository.Delete(user);
        }

        public  async Task<IdentityResult> Update(AppUser user)
        {
            return await _repository.Update(user);
        }

        public UserManager<AppUser> GetUserManager()
        {
            return _repository.GetUserManager();
        }

    }
}
