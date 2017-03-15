using System.Linq;
using System.Threading.Tasks;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IQueryable<AppUser> Get() => _userManager.Users;

        public AppUser GetByEmail(string email) => _userManager.Users.First(u => u.Email == email);

        public Task<IdentityResult> Create(AppUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> Delete(AppUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> Update(AppUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public UserManager<AppUser> GetUserManager()
        {
            return  _userManager;
        }
    }
}
