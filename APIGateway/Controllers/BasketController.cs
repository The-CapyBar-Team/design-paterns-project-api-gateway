using AllProductsService.Protos;
using APIGateway.Services;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private RabbitMQService rabbitMQService;
        public BasketController(RabbitMQService rabbitMQService)
        {
            this.rabbitMQService = rabbitMQService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddToCartRequest request)
        {
            await rabbitMQService.SendMessage("AddToCartRequests", request.ToByteArray());
            return Ok();
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveProductFromCart([FromBody] RemoveFromCartRequest request)
        {
            await rabbitMQService.SendMessage("RemoveFromCartRequests", request.ToByteArray());
            return Ok();
        }

        [HttpPost("status")]
        public async Task<IActionResult> GetProductStatusFromCart([FromBody] ProductsStatusRequest request)
        {
            await rabbitMQService.SendMessage("ProductStatusRequests", request.ToByteArray());
            return Ok();
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyProductFromCart([FromBody] BuyProductRequest request)
        {
            await rabbitMQService.SendMessage("BuyProductRequests", request.ToByteArray());
            return Ok();
        }
    }
}
