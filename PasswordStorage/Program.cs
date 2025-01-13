using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ApiGateway.Models;
using PasswordStorage;


var a = new UserHandler();
await a.TestMethod();

