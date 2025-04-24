using AllProductsService.Protos;
using APIGateway.Services;
using APIGateway.Utils;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllProductsController : ControllerBase
    {
        private RabbitMQService rabbitMQService;
        private ResponseTracker responseTracker;
        public AllProductsController(RabbitMQService rabbitMQService, ResponseTracker responseTracker) 
        {
            this.rabbitMQService = rabbitMQService;
            this.responseTracker = responseTracker;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var correlationId = Guid.NewGuid().ToString();
            responseTracker.RegisterRequest(correlationId);

            var request = new AllProductsRequest();
            request.RequestId = correlationId;

            responseTracker.RegisterRequest(correlationId);

            await rabbitMQService.SendMessage("AllproductsRequests", request.ToByteArray());

            var resp = await responseTracker.AwaitResponseAsync(correlationId);

            if (resp == null)
                return StatusCode(500, "Internal server error");
            var products = AllProductsResponse.Parser.ParseFrom(resp);

            return Ok(products.Products.ToList());
        }
    }
}
