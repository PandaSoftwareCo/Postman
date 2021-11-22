using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Postman.Models
{
    public class TokenModel
    {
        public string token { get; set; }

        //[JsonProperty("success")]
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
