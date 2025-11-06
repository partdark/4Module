

using Application.DTO;
using Application.Interfaces;
using Domain.Interfaces;
using MassTransit;
using MassTransit.Internals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Core.Operations;
using OrderContracts;
using System.Security.Cryptography.Xml;

namespace _4Module.Controllers
{
    /// <summary>
    /// Controller for orders
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]


    public class OrderController : ControllerBase
    {

        private readonly IBus _bus;


        public OrderController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]

        public async Task<IActionResult> Publish([FromBody] SubmitOrderCommand submitOrderCommand)
        {
       
            await _bus.Publish(submitOrderCommand);
            return Accepted();

        }




    }
}
