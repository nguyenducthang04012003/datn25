using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.Carts;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //api get all cart list
        [HttpGet("GetAllCartList")]
        public async Task<IActionResult> GetCartList()
        {
            var response = await _cartService.GetCartList();
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api add to cart
        [HttpPost("AddToCart")]
        public async Task <IActionResult> AddToCart([FromForm]CartModelDto cartModelDto)
        {
            var response = await _cartService.AddToCart(cartModelDto);

            if (!ModelState.IsValid)
                return BadRequest(new { response.Message });

            if (!response.Success) return BadRequest(new { response.Message });

            return Ok(response);
        }


        // api update quantity
        [HttpPut("UpdateCart/{productId}/{quantity}")]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            var response = await _cartService.UpdateCart(productId, quantity);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api remove item from cart
        [HttpDelete("RemoveFromCart/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var response = await _cartService.RemoveFromCart(productId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

    }
}
