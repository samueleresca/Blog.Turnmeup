using System;
using System.ComponentModel.DataAnnotations;

namespace Blog.Turnmeup.DL.Models
{
    public class BaseResponseModel
    {
        [Required]
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
    }
}
