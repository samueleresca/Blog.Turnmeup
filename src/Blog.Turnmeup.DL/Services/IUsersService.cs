using System.Linq;
using System.Threading.Tasks;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Services
{
    public interface IUsersService
    {
        IQueryable<UserResponseModel> Get();
        UserResponseModel GetByEmail(string email);
        Task<IdentityResult> Create(UserResponseModel user, string password);
        Task<IdentityResult> Delete(UserResponseModel user);
        Task<IdentityResult> Update(UserResponseModel user);
        Task<IdentityResult> ValidatePassword(UserResponseModel user, string password);
        Task<IdentityResult> ValidateUser(UserResponseModel user);
        string HashPassword(UserResponseModel user, string password);
        Task SignOutAsync();
        Task<SignInResult> PasswordSignInAsync(UserResponseModel user, string password, bool lockoutOnFailure,
            bool isPersistent);
    }
}
