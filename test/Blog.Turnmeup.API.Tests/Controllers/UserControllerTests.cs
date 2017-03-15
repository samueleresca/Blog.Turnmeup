using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.API.Controllers;
using Blog.Turnmeup.API.Models.Users;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Repositories;
using Blog.Turnmeup.DL.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Blog.Turnmeup.API.Tests.Controllers
{
    public class UserControllerTests : IClassFixture<TestFixture<Startup>>
    {

        private IUsersRepository Repository { get; set; }

        private IUsersService Service { get; }

        private UsersController Controller { get; }

        public UserControllerTests(TestFixture<Startup> fixture)
        {

            var users = new List<AppUser>
            {
                new AppUser
                {
                    UserName = "Test",
                    Id = Guid.NewGuid().ToString(),
                    Email = "test@test.it"
                }

            }.AsQueryable();

            var fakeUserManager = new Mock<FakeUserManager>();

            fakeUserManager.Setup(x => x.Users)
                .Returns(users);

            fakeUserManager.Setup(x => x.DeleteAsync(It.IsAny<AppUser>()))
             .ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
          .ReturnsAsync(IdentityResult.Success);


            Repository = new UsersRepository(fakeUserManager.Object);


            var mapper = (IMapper)fixture.Server.Host.Services.GetService(typeof(IMapper));
            var errorHandler = (IErrorHandler)fixture.Server.Host.Services.GetService(typeof(IErrorHandler));
            var uservalidator = (IUserValidator<AppUser>)fixture.Server.Host.Services.GetService(typeof(IUserValidator<AppUser>));
            var passwordvalidator = (IPasswordValidator<AppUser>)fixture.Server.Host.Services.GetService(typeof(IPasswordValidator<AppUser>));
            var passwordhasher = (IPasswordHasher<AppUser>)fixture.Server.Host.Services.GetService(typeof(IPasswordHasher<AppUser>));
            var signInManager = (SignInManager<AppUser>)fixture.Server.Host.Services.GetService(typeof(SignInManager<AppUser>));

            //SERVICES CONFIGURATIONS
            Service = new UsersService(Repository, mapper, uservalidator, passwordvalidator, passwordhasher, signInManager);
            Controller = new UsersController(Service, errorHandler);
        }



        [Theory]
        [InlineData("test@test.it", "Ciao.Ciao", "Test_user")]
        public async Task Insert(string email, string password, string name)
        {
            //Arrange
            var testUser = new CreateRequestModel
            {
                Email = email,
                Name = name,
                Password = password
            };
            //Act
            var createdUser = await Controller.Create(testUser);
            //Assert
            Assert.Equal(email, createdUser.Email);
        }

        [Fact]
        public void Get()
        {
            //Act
            var result = Controller.Get();
            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("test@test.it")]
        public void GetByEmail(string email)
        {
            //Act
            var result = Controller.Get(email);
            // Assert
            Assert.Equal(result.Email, email);
        }



        [Theory]
        [InlineData("test@test.it", "password", "Test")]
        public async Task Update(string email, string password, string name)
        {
            //Act
            var updated = await Controller.Edit(email, password);
            // Assert
            Assert.Equal(email, updated.Email);
        }

        [Theory]
        [InlineData("test@test.it", "Ciao.Ciao")]
        public async Task Delete(string email, string password)
        {
            //Arrange
            var testUser = new DeleteRequestModel
            {
                Email = email,
                Password = password
            };
            //Act
            var deleted = await Controller.Delete(testUser);
            //Assert
            Assert.Equal(email, deleted.Email);
        }
    }
}
