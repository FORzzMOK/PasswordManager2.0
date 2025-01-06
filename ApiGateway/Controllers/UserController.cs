using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ApiGateway.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Controllers;

[ApiController]
public class UserController : Controller
{
    private readonly IConfiguration _configuration;
    
    public UserController(IConfiguration config)
    {
        _configuration = config;
    }
    
    private static readonly IEnumerable<User> People =
    [
        new() { Login = "admin@gmail.com", Password = "12345", Role = "admin" },
        new() { Login = "qwerty@gmail.com", Password = "55555", Role = "user" }
    ];
    
    public record Token(string JwtToken, string Response);
    
    [AllowAnonymous]
    [HttpGet(nameof(GetToken))]
    public Token GetToken(string login, string password)
    {
        if (!login.IsNullOrEmpty() || !password.IsNullOrEmpty())
        {
            var user = People.SingleOrDefault(i => i.Login == login && i.Password == password);

            if (user != null)
            {
                var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? throw new InvalidOperationException()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                };
                
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException()));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(100),
                    signingCredentials: signIn);
                
                return new Token($"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}", "Ok");
            }
            else
            {
                return new Token("", "Invalid credentials");
            }
        }
        else
        {
            return new Token("","Invalid credentials");
        }
    }

    [Authorize]
    [HttpGet(nameof(GetAllUsers))]
    public IEnumerable<User> GetAllUsers()
    {
        return People;   
    }
}