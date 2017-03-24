using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Turnmeup.DL.Models;

namespace Blog.Turnmeup.API.Models.Users
{
    public class UpdateRequestModel : UserModel
    {
        [Required]
        public string Password { get; set; }
    }
}
