using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blog.Turnmeup.API.Models.Courses
{
    public class WhereRequestModel
    {
        public IDictionary<string, string> Criterias { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
