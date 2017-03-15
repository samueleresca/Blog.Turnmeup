using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.API.Models.Courses;
using Blog.Turnmeup.DL.Infrastructure.ErrorHandler;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Services;
using Blog.Turnmeup.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Blog.Turnmeup.API.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {

        private readonly ICourseService _service;
        private readonly IErrorHandler _errorHandler;
        
        public CoursesController(ICourseService service, IMapper mapper, IErrorHandler errorHandler)
        {
            _service = service;
            _errorHandler = errorHandler;
        }


        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<CourseResponseModel>> Get()
        {

            return await _service.GetAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<CourseResponseModel> Get([Required]int id)
        {
            return await _service.GetById(id);
        }

        [HttpGet("{fieldLabel}/{fieldValue}")]
        public List<CourseResponseModel> Where(string fieldLabel, string fieldValue)
        {

            return
                _service.Where(
                    entity => (string) entity.GetType().GetProperty(fieldLabel).GetValue(entity, null) == fieldValue).ToList();

        }

        [HttpGet("where/criterias/{criteriasString}")]
        public List<CourseResponseModel> Where(string criteriasString)
        {

            var criteriasModel = JsonConvert.DeserializeObject<WhereRequestModel>(criteriasString);
            var whereResult = _service.GetAsync().Result;
            whereResult = criteriasModel.Criterias.Aggregate(whereResult, (current, attribute) => current.Where(entity => (string) entity.GetType().GetProperty(attribute.Key).GetValue(entity, null) == attribute.Value).AsEnumerable());
            return whereResult.ToList();
        }
        // POST api/values
        [HttpPost]
        public void Post([FromBody]CourseResponseModel entity)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation), "", ModelState.Values.First().Errors.First().ErrorMessage));
            }

            _service.AddOrUpdate(entity);
        }


        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete([Required]int id)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation), "", ModelState.Values.First().Errors.First().ErrorMessage));
            }

            _service.Remove(id);
        }
    }
}
