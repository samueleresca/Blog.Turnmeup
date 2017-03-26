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

            var users = new List<UserModel>
            {
                new UserModel
                {
                    UserName = "Test",
                    Id = Guid.NewGuid().ToString(),
                    Email = "test@test.it"
                }

            }.AsQueryable();

            var fakeUserManager = new Mock<FakeUserManager>();

            fakeUserManager.Setup(x => x.Users)
                .Returns(users);

            fakeUserManager.Setup(x => x.DeleteAsync(It.IsAny<UserModel>()))
             .ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<UserModel>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
            fakeUserManager.Setup(x => x.UpdateAsync(It.IsAny<UserModel>()))
          .ReturnsAsync(IdentityResult.Success);


            Repository = new UsersRepository(fakeUserManager.Object);


            var mapper = (IMapper)fixture.Server.Host.Services.GetService(typeof(IMapper));
            var errorHandler = (IErrorHandler)fixture.Server.Host.Services.GetService(typeof(IErrorHandler));
            var passwordhasher = (IPasswordHasher<AppUser>)fixture.Server.Host.Services.GetService(typeof(IPasswordHasher<AppUser>));


            var uservalidator = new Mock<IUserValidator<AppUser>>();
            uservalidator.Setup(x => x.ValidateAsync(It.IsAny<UserManager<AppUser>>(), It.IsAny<AppUser>()))
             .ReturnsAsync(IdentityResult.Success);
            var passwordvalidator = new Mock<IPasswordValidator<AppUser>>();
            passwordvalidator.Setup(x => x.ValidateAsync(It.IsAny<UserManager<AppUser>>(), It.IsAny<AppUser>(), It.IsAny<string>()))
             .ReturnsAsync(IdentityResult.Success);

            var signInManager = new Mock<FakeSignInManager>();

            signInManager.Setup(
                    x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);


            //SERVICES CONFIGURATIONS
            Service = new UsersService(Repository, mapper, uservalidator.Object, passwordvalidator.Object, passwordhasher, signInManager.Object);
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
            //Arrange
            var testUser = new UpdateRequestModel
            {
                Email = email,
                Password = password
            };

            //Act
            var updated = await Controller.Edit(testUser);
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

        [Theory]
        [InlineData("test@test.it", "Ciao.Ciao")]
        public async Task TokenAsync(string email, string password)
        {
            //Arrange
            var testUser = new TokenRequestModel
            {
                Username = email,
                Password = password
            };

            //Act
            var updated = await Controller.Token(testUser);
            // Assert
            Assert.Equal("Test", updated.Principal.Identity.Name);
           
        }
    }
}
