using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using ApiGateway.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGateway.RabbitMq;

namespace ApiGateway.Controllers;

[ApiController]
public class RabbitMqController : Controller
{
    private readonly IRabbitMqService _mqService;
    
    public RabbitMqController(IRabbitMqService mqService)
    {
        _mqService = mqService;
    }
    
    [AllowAnonymous]
    [HttpGet(nameof(TestRabbitMqController))]
    public string TestRabbitMqController()
    {
        _mqService.SendMessage("Test RabbitMqController");
        return "Ok";
    }
}