using APIGateway.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpPost("add")]
        public IActionResult AddProductToCart([FromBody] BasketRequest request)
        {
            return Ok();
        }

        [HttpPost("remove")]
        public IActionResult RemoveProductFromCart([FromBody] BasketRequest request)
        { 
            return Ok();
        }

        [HttpPost("status")]
        public IActionResult GetProductStatusFromCart([FromBody] BasketRequest request)
        {
            return Ok();
        }

        [HttpPost("buy")]
        public IActionResult BuyProductFromCart([FromBody] BasketRequest request)
        {
            return Ok();
        }
    }
}
