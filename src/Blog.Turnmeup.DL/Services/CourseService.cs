using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.Models;

namespace Blog.Turnmeup.DL.Services
{
    public class CourseService :  ICourseService
    {
        private readonly IBaseService<Course> _service;
        private readonly IMapper _mapper;


        public CourseService(IBaseService<Course> service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseResponseModel>> GetAsync()
        {
            var result = await _service.GetAsync();
            return result.Select(t => _mapper.Map<Course, CourseResponseModel>(t));
        }

        public async Task<CourseResponseModel> GetById(int id)
        {
            return _mapper.Map<Course, CourseResponseModel>(await _service.GetById(id));
        }

        public IEnumerable<CourseResponseModel> Where(Expression<Func<Course, bool>> exp)
        {
            var whereResult = _service.Where(exp).ToList();
            return _mapper.Map<List<Course>, List<CourseResponseModel>>(whereResult).AsEnumerable();
        }

        public void AddOrUpdate(CourseResponseModel entry)
        {
            _service.AddOrUpdate(_mapper.Map<CourseResponseModel, Course>(entry));
        }

        public void Remove(int id)
        {
            _service.Remove(id);
        }
    }
}
