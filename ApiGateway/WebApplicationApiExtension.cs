using System.Globalization;
using ApiGateway.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#nullable enable

namespace ApiGateway;

[AllowAnonymous]
[ApiController]
public class HelloWorldController : Controller
{
    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
    
    private static readonly string[] Summaries = 
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    [AllowAnonymous]
    [HttpGet(nameof(GetWeatherForecast))]
    public IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    Summaries[Random.Shared.Next(Summaries.Length)]
                ))
            .ToArray();
        return forecast;
    }
}

public static class WebApplicationApiExtension
{
    private static readonly List<User> People =
    [
        new User { Login = "admin@gmail.com", Password = "12345", Role = Roles.Admin },
        new User { Login = "qwerty@gmail.com", Password = "55555", Role = Roles.User }
    ];

    private record Token(string JwtToken, string Response);
    
    public static IApplicationBuilder AddUserApi(this WebApplication app)
    {
        app.MapGet("/token", new Func<string, string, Token>((string login, string password) =>
            {
                if (login.IsNullOrEmpty() && password.IsNullOrEmpty())
                {
                    var user = People.SingleOrDefault(i => i.Login == login && i.Password == password);

                    if (user != null)
                    {
                        var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Sub, app.Configuration["Jwt:Subject"] ?? throw new InvalidOperationException()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(app.Configuration["Jwt:Key"] ?? throw new InvalidOperationException()));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            app.Configuration["Jwt:Issuer"],
                            app.Configuration["Jwt:Audience"],
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
            }))
            .WithName("GetToken")
            .WithOpenApi();
        
        return app;
    }
}