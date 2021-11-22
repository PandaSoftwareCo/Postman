using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostmanApi.Interfaces;

namespace PostmanApi.Authentication.Jwt
{
    public class AuthenticationSettings : IAuthenticationSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int ExpirationDays { get; set; }
    }
}
