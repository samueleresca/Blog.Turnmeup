
namespace Blog.Turnmeup.Models
{
    public class Course : BaseEntity
    {
      
        public string Url { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string Tags { get; set; }

        public byte[] Image { get; set; }
    }
}
