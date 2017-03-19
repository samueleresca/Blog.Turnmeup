using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.API.Controllers;
using Blog.Turnmeup.API.Models.Courses;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Repositories;
using Blog.Turnmeup.DL.Services;
using Blog.Turnmeup.Models;
using Moq;
using Xunit;

namespace Blog.Turnmeup.API.Tests.Controllers
{
    public class CoursesControllerTests : IClassFixture<TestFixture<Startup>>
    {

        private Mock<IBaseRepository<Course>> Repository { get; }

        private ICourseService Service { get; }

        private CoursesController Controller { get; }

        public CoursesControllerTests(TestFixture<Startup> fixture)
        {
            //Arrange
            var entities = new List<Course>
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
                .ReturnsAsync(entities);

            Repository.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((int id) => Task.Run(() => entities.Find(t => t.Id == id)));

            Repository.Setup(x => x.Where(It.IsAny<Expression<Func<Course, bool>>>()))
                    .Returns((Expression<Func<Course, bool>> exp) => entities.AsQueryable().Where(exp));

            Repository.Setup(x => x.Insert(It.IsAny<Course>()))
                .Callback((Course entity) => entities.Add(entity));

            Repository.Setup(x => x.Update(It.IsAny<Course>()))
                .Callback((Course entity) => entities[entities.FindIndex(x => x.Id == entity.Id)] = entity);

            Repository.Setup(x => x.Delete(It.IsAny<Course>()))
                .Callback((Course entity) => entities.RemoveAt(entities.FindIndex(x => x.Id == entity.Id)));


            var imapper = (IMapper)fixture.Server.Host.Services.GetService(typeof(IMapper));
            var ierrorHandler = (IErrorHandler)fixture.Server.Host.Services.GetService(typeof(IErrorHandler));

            //SERVICES CONFIGURATIONS
            var baseService= new BaseService<Course>(Repository.Object);
            Service = new CourseService(baseService, imapper);
            Controller = new CoursesController(Service, imapper, ierrorHandler);
        }


        [Fact]
        public void GetLabelsController()
        {
            //Act
            var result = Controller.Get().Result;
            // Assert
            Repository.Verify(x => x.GetAll(), Times.Once);
            Assert.Equal(result.Count(), 1);
        }

        [Theory]
        [InlineData(1)]
        public void GetLabelsByIdController(int id)
        {
            //Act
            var result = Controller.Get(id);
            // Assert
            Repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            Assert.Equal(result.Id, id);
        }

        [Theory]
        [InlineData("Title", "Test")]
        [InlineData("Url", "https://samueleresca.net/")]
        public void WhereLabelController(string labelField, string labelValue)
        {
            //Act
            var result = Controller.Where(labelField, labelValue).First();
            // Assert
            Repository.Verify(x => x.Where(It.IsAny<Expression<Func<Course, bool>>>()), Times.Once);
            Assert.Equal(labelValue, (string)result.GetType().GetProperty(labelField).GetValue(result, null));
        }

        [Fact]
        public void WhereMultipleCriteriaLabelController()
        {
            var filterCriteria = new WhereRequestModel { Criterias = new Dictionary<string, string> {{"Title", "Test"}, {"Url", "https://samueleresca.net/"}}};

            //Act
            var result = Controller.Where(filterCriteria.ToString()).First();
            // Assert
            Assert.Equal(filterCriteria.Criterias.ElementAt(0).Value, (string)result.GetType().GetProperty(filterCriteria.Criterias.ElementAt(0).Key).GetValue(result, null));
            Assert.Equal(filterCriteria.Criterias.ElementAt(1).Value, (string)result.GetType().GetProperty(filterCriteria.Criterias.ElementAt(1).Key).GetValue(result, null));
        }
        
        [Theory]
        [InlineData(1,  "Test 2", "https://samueleresca.net")]
        public void UpdateLabel(int id, string title, string url)
        {
            //Arrange
            var toUpdate = new CourseResponseModel
            {
                Id = id,
                Title= title,
                Url= url
            };
            //Act
            Controller.Post(toUpdate);
            // Assert
            Repository.Verify(x => x.Update(It.IsAny<Course>()), Times.Once);
            var result = Controller.Get(id).Result;
            Assert.Equal(title, result.Title);
        }

        [Theory]
        [InlineData(2,  "Test 2", "https://samueleresca.net")]
        public void InsertLabel(int id, string title, string url)
        {
            //Arrange
            var entity = new CourseResponseModel
            {
                Id = id,
                Title = title,
                Url = url
            };
            //Act
            Controller.Post(entity);
            //Assert
            var result = Controller.Get().Result;
            Repository.Verify(x => x.Insert(It.IsAny<Course>()), Times.Once);
            Assert.Equal(2, result.Count());
        }

        [Theory]
        [InlineData(1)]
        public void DeleteLabel(int id)
        {
            //Act
            Controller.Delete(id);
            //Assert
            Repository.Verify(x => x.Delete(It.IsAny<Course>()), Times.Once);
            var result = Controller.Get().Result;
            Assert.Equal(0, result.Count());
        }
    }
}
