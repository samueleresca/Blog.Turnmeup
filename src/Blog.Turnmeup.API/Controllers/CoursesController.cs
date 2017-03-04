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

        private readonly IBaseService<Course> _service;
        private readonly IMapper _mapper;
        private readonly IErrorHandler _errorHandler;

        public CoursesController(IBaseService<Course> service, IMapper mapper, IErrorHandler errorHandler)
        {
            _service = service;
            _mapper = mapper;
            _errorHandler = errorHandler;
        }


        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<CourseResponseModel>> Get()
        {

            var result = await _service.Get();
            return result.Select(t => _mapper.Map<Course, CourseResponseModel>(t));
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<CourseResponseModel> Get([Required]int id)
        {
            return _mapper.Map<Course, CourseResponseModel>(await _service.GetById(id));
        }

        [HttpGet("{fieldLabel}/{fieldValue}")]
        public List<CourseResponseModel> Where(string fieldLabel, string fieldValue)
        {
            var whereResult = _service.Where(entity => (string)entity.GetType().GetProperty(fieldLabel).GetValue(entity, null) == fieldValue).ToList();
            return _mapper.Map<List<Course>, List<CourseResponseModel>>(whereResult);
        }

        [HttpGet("where/criterias/{criteriasString}")]
        public List<CourseResponseModel> Where(string criteriasString)
        {

            var criteriasModel = JsonConvert.DeserializeObject<WhereRequestModel>(criteriasString);
            var whereResult = _service.Get().Result;
            whereResult = criteriasModel.Criterias.Aggregate(whereResult, (current, attribute) => current.Where(entity => (string) entity.GetType().GetProperty(attribute.Key).GetValue(entity, null) == attribute.Value).AsEnumerable());

            return _mapper.Map<List<Course>, List<CourseResponseModel>>(whereResult.ToList());
        }
        // POST api/values
        [HttpPost]
        public void Post([FromBody]CourseResponseModel entity)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpRequestException(string.Format(_errorHandler.GetMessage(ErrorMessagesEnum.ModelValidation), "", ModelState.Values.First().Errors.First().ErrorMessage));
            }

            _service.AddOrUpdate(_mapper.Map<CourseResponseModel, Course>(entity));
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
