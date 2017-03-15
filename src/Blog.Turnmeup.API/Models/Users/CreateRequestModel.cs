using System.ComponentModel.DataAnnotations;

namespace Blog.Turnmeup.API.Models.Users
{
    public class CreateRequestModel
    {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
    }
}
