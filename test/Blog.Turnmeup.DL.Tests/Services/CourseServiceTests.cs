using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.API;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Repositories;
using Blog.Turnmeup.DL.Services;
using Blog.Turnmeup.Models;
using Moq;
using Xunit;


namespace Blog.Turnmeup.DL.Tests.Services
{
    public class CourseServiceTest : IClassFixture<TestFixture<Startup>>
    {

        private Mock<IBaseRepository<Course>> Repository { get; }

        private ICourseService Service { get; }


        public CourseServiceTest(TestFixture<Startup> fixture)
        {


            var entity = new List<Course>
            {
                new Course
                {
                    Id = 1,
                    Title = "Test",
                    Url = "https://samueleresca.net/",
                    DateAdded = DateTime.Now
                }
            };

            Repository = new Mock<IBaseRepository<Course>>();

            Repository.Setup(x => x.GetAll())
                .ReturnsAsync(entity);

            Repository.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((int id) => Task.Run(() => entity.Find(s => s.Id == id)));


            Repository.Setup(x => x.Where(It.IsAny<Expression<Func<Course, bool>>>()))
                .Returns((Expression<Func<Course, bool>> exp) => entity.AsQueryable().Where(exp));


            Repository.Setup(x => x.Insert(It.IsAny<Course>()))
                .Callback((Course label) => entity.Add(label));

            Repository.Setup(x => x.Update(It.IsAny<Course>()))
                .Callback((Course label) => entity[entity.FindIndex(x => x.Id == label.Id)] = label);

            Repository.Setup(x => x.Delete(It.IsAny<Course>()))
            .Callback((Course label) => entity.RemoveAt(entity.FindIndex(x => x.Id == label.Id)));

            var mapper = (IMapper)fixture.Server.Host.Services.GetService(typeof(IMapper));
            var baseService = new BaseService<Course>(Repository.Object);


            Service = new CourseService(baseService, mapper);
        }

        [Fact]
        public void Can_Get_All()
        {
            // Act
            var entities = Service.GetAsync().Result;
            // Assert
            Repository.Verify(x => x.GetAll(), Times.Once);
            Assert.Equal(1, entities.Count());
        }

        [Fact]
        public void Can_Get_Single()
        {
            // Arrange
            var testId = 1;

            // Act
            var l = Service.GetById(testId).Result;

            // Assert
            Repository.Verify(x => x.GetById(testId), Times.Once);
            Assert.Equal("Test", l.Title);
            Assert.Equal("https://samueleresca.net/", l.Url);
        }

        [Fact]
        public void Can_Filter_Entities()
        {
            // Arrange
            var courseId = 1;

            // Act
            var filteredEntities = Service.Where(s => s.Id == courseId).First();

            // Assert
            Repository.Verify(x => x.Where(s => s.Id == courseId), Times.Once);
            Assert.Equal(courseId, filteredEntities.Id);
            Assert.Equal("Test", filteredEntities.Title);
        }

        [Fact]
        public void Can_Insert_Entity()
        {
            // Arrange
            var entity = new CourseResponseModel
            {
                Id = 2,
                Title = "Course 2",
                DateAdded = DateTime.MaxValue
            };

            // Act
            Service.AddOrUpdate(entity);


            // Assert
            Repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Repository.Verify(x => x.Insert(It.IsAny<Course>()), Times.Once);
            var entities = Service.GetAsync().Result;
            Assert.Equal(2, entities.Count());
        }


        [Fact]
        public void Can_Update_Entity()
        {
            // Arrange
            var entity = new CourseResponseModel
            {
                Id = 1,
                Title = "Course 2 ",
                Url = "https://samueleresca.net"
            };
            // Act
            Service.AddOrUpdate(entity);

            // Assert
            Repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Repository.Verify(x => x.Update(It.IsAny<Course>()), Times.Once);
            var entityResult = Service.GetById(1).Result;
            Assert.Equal("Course 2 ", entityResult.Title);
            Assert.Equal(DateTime.UtcNow.Date, entityResult.DateModified.Date);
        }

    }
}
