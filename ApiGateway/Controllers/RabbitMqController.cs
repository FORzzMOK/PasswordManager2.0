using ApiGateway.Events;
using Libraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
public class RabbitMqController : Controller
{
    private readonly IRabbitMqService<UserEvent> _mqService;
    
    public RabbitMqController(IRabbitMqService<UserEvent> mqService)
    {
        _mqService = mqService;
    }
    
    [AllowAnonymous]
    [HttpGet(nameof(TestRabbitMqController))]
    public string TestRabbitMqController()
    {
        var user = UserController.People.FirstOrDefault();
        
        var userEvent = new UserEvent() { Login = user.Login, Password = user.Password, Role = user.Role };
        _mqService.SendMessage(userEvent);
        return "Ok";
    }
}