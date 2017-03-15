using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
      

        public UsersService(IUsersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<UserResponseModel> Get()
        {

            var returnedList = new List<UserResponseModel>();
            _repository.Get().ToList().ForEach(u =>
            {
                returnedList.Add(_mapper.Map<AppUser, UserResponseModel>(u));
            });

            return returnedList.AsQueryable();
        }

        public UserResponseModel GetByEmail(string email)
        {
            return _mapper.Map<AppUser, UserResponseModel>(_repository.GetByEmail(email));
        }

        public Task<IdentityResult> Create(UserResponseModel user, string password)
        {
            return _repository.Create(user, password);
        }

        public async Task<IdentityResult> Delete(UserResponseModel user)
        {
            return await _repository.Delete(user);
        }

        public  async Task<IdentityResult> Update(UserResponseModel user)
        {
            return await _repository.Update(user);
        }

        public UserManager<UserResponseModel> GetUserManager()
        {
            return _mapper.Map<UserManager<AppUser>, UserManager<UserResponseModel>>(_repository.GetUserManager());
        }

    }
}
