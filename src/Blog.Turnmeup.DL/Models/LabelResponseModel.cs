using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Turnmeup.DL.Models;

namespace Blog.Turnmeup.DL.Models
{
    [ModelBinder]
    public class CourseResponseModel : BaseResponseModel
    {
        
        public string Url { get; set; }

        public string Description { get; set; }

        
        public string Title { get; set; }

        public string Tags { get; set; }

        public byte[] Image { get; set; }
    }
}
