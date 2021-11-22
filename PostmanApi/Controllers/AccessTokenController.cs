using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostmanApi.Authentication.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using PostmanApi.Interfaces;
using AutoMapper.Configuration;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PostmanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessTokenController : ControllerBase
    {
        private readonly AuthenticationSettings _authenticationSettings;
        public IConfiguration Configuration { get; }
        private readonly ILogger<AccessTokenController> _logger;

        public AccessTokenController(IAuthenticationSettings settings, IOptions<AuthenticationSettings> authenticationSettings, IConfiguration config, ILogger<AccessTokenController> logger)
        {
            _authenticationSettings = authenticationSettings.Value;
        }

        // GET: api/<AccessTokenController>
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Basic")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public IActionResult Get()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = User.Identity as ClaimsIdentity;
            var userName = identity.Name;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_authenticationSettings.Issuer, _authenticationSettings.Audience, claims, 
                expires: DateTime.UtcNow.AddDays(_authenticationSettings.ExpirationDays), signingCredentials: creds);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }

        // POST api/<AccessTokenController>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = User.Identity as ClaimsIdentity;
            //var userName = userName;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_authenticationSettings.Issuer, _authenticationSettings.Audience, claims, expires: DateTime.Now.AddDays(_authenticationSettings.ExpirationDays), signingCredentials: creds);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}
