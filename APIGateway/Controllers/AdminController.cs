using AllProductsService.Protos;
using APIGateway.Services;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private RabbitMQService rabbitMQService;
        public AdminController(RabbitMQService rabbitMQService)
        {
            this.rabbitMQService = rabbitMQService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToDb([FromBody] ProductProto product)
        {
            await rabbitMQService.SendMessage("AddProductDBRequests", product.ToByteArray());
            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> RedactProductInDb([FromBody] ProductProto product)
        {
            await rabbitMQService.SendMessage("UpdateProductDBRequests", product.ToByteArray());
            return Ok();
        }
    }
}
