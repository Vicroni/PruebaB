using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MOKBack.Models;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MOKBack.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private IConfiguration configuration;
        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public string Index()
        {
            return "Genera tu token y utiliza postman";
        }

        [HttpGet]
        [Route("login")]
        public IResult Login([FromBody]User user)
        {
            if (user.Username == "saul" && user.Password == "1234")
            {
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes
                (configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
            }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var stringToken = tokenHandler.WriteToken(token);
                return Results.Ok(stringToken);
            }
            return Results.Unauthorized();
        }
    }
}


