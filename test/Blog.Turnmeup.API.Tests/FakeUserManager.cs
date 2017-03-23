using System;
using System.Threading.Tasks;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Blog.Turnmeup.API.Tests
{
    public class FakeUserManager : UserManager<AppUser>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<AppUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<AppUser>>().Object,
                  new[] {new Mock<IUserValidator<AppUser>>().Object},
                    new[] { new Mock<IPasswordValidator<AppUser>>().Object },
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<AppUser>>>().Object)
        { }


    }
}
