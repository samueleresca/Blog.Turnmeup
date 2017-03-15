using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Blog.Turnmeup.API;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Repositories;
using Blog.Turnmeup.DL.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Blog.Turnmeup.DL.Tests.Services
{
    public class UserServicesTest : IClassFixture<TestFixture<Startup>>
    {

        private Mock<IUsersRepository> Repository { get; }

        private IUsersService Service { get; }

        private static readonly string UniqueId = Guid.NewGuid().ToString();

        public UserServicesTest(TestFixture<Startup> fixture)
        {


            var users = new List<AppUser>
            {
                new AppUser
                {
                    Id = UniqueId,
                    UserName = "testusername",
                    Email = "test@test.it"

                }
            };

            Repository = new Mock<IUsersRepository>();

            Repository.Setup(x => x.Get())
                .Returns(users.AsQueryable());

            Repository.Setup(x => x.GetByEmail(It.IsAny<string>()))
                .Returns((string email) => users.Find(s => s.Email == email));

            Repository.Setup(x => x.Create(It.IsAny<AppUser>(), It.IsAny<string>()))
                .Callback((AppUser user, string password) => users.Add(user));

            Repository.Setup(x => x.Update(It.IsAny<AppUser>()))
                .Callback((AppUser user) => users[users.FindIndex(x => x.Id == user.Id)] = user);

            Repository.Setup(x => x.Delete(It.IsAny<AppUser>()))
            .Callback((AppUser label) => users.RemoveAt(users.FindIndex(x => x.Id == label.Id)));


            var mapper = (IMapper)fixture.Server.Host.Services.GetService(typeof(IMapper));
            var errorHandler = (IErrorHandler)fixture.Server.Host.Services.GetService(typeof(IErrorHandler));
            var uservalidator = (IUserValidator<AppUser>)fixture.Server.Host.Services.GetService(typeof(IUserValidator<AppUser>));
            var passwordvalidator = (IPasswordValidator<AppUser>)fixture.Server.Host.Services.GetService(typeof(IPasswordValidator<AppUser>));
            var passwordhasher = (IPasswordHasher<AppUser>)fixture.Server.Host.Services.GetService(typeof(IPasswordHasher<AppUser>));
            var signInManager = (SignInManager<AppUser>)fixture.Server.Host.Services.GetService(typeof(SignInManager<AppUser>));
            var userManager = (UserManager<AppUser>)fixture.Server.Host.Services.GetService(typeof(UserManager<AppUser>));

            //SERVICES CONFIGURATIONS
            Service = new UsersService(Repository.Object, mapper, uservalidator, passwordvalidator, passwordhasher, signInManager);
        }

        [Fact]
        public void GetAllUsers()
        {
            // Act
            var users = Service.Get();
            // Assert
            Repository.Verify(x => x.Get(), Times.Once);
            Assert.Equal(1, users.Count());
        }


        [Theory]
        [InlineData("test@test.it")]
        public void GetSingleLabel(string userEmail)
        {
            // Act
            var user = Service.GetByEmail(userEmail);

            // Assert
            Repository.Verify(x => x.GetByEmail(userEmail), Times.Once);
            Assert.Equal(userEmail, user.Email);
            Assert.Equal("testusername", user.UserName);
        }



        [Fact]
        public void InsertUser()
        {
            // Arrange
            var user = new UserResponseModel
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testusername2",
                Email = "test2@test.it"
            };
            // Act
            Service.Create(user, "testpassword");
            // Assert
            Repository.Verify(x => x.Create(It.IsAny<AppUser>(), It.IsAny<string>()), Times.Once);
            var labels = Service.Get();
            Assert.Equal(2, labels.Count());
        }


        [Fact]
        public void UpdateUser()
        {
            // Arrange
            var user = new UserResponseModel
            {
                Id = UniqueId,
                UserName = "testusername",
                Email = "test@test.it",
                PhoneNumber = "test"

            };
            // Act
            Service.Update(user);
            // Assert
            Repository.Verify(x => x.Update(It.IsAny<AppUser>()), Times.Once);
            var labels = Service.Get();
            Assert.Equal(1, labels.Count());
            Assert.Equal("test", labels.First().PhoneNumber);
        }

        [Fact]
        public void DeleteUser()
        {
            const string email = "test@test.it";
            
            // Act
            var user = Service.GetByEmail(email);
            Service.Delete(user);
            
            // Assert
            Repository.Verify(x => x.GetByEmail(It.IsAny<string>()), Times.Once);
            Repository.Verify(x => x.Delete(It.IsAny<AppUser>()), Times.Once);
            Assert.Equal(0, Service.Get().Count());
        }



    }
}
