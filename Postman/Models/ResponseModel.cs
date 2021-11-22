using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postman.Models
{
    public class ResponseModel
    {
        public string Reference { get; set; }
        public List<string> Errors { get; set; }
    }
}
