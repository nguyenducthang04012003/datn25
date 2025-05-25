using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.ProductLots;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.Models;
using PharmaDistiPro.Services;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductLotController : ControllerBase
    {
        private readonly IProductLotService _productLotService;

        public ProductLotController(IProductLotService productLotService)
        {
            _productLotService = productLotService;
        }
        [HttpGet]
        public async Task<ActionResult<List<ProductLotResponse>>> GetProductLots()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _productLotService.GetProductLotList();
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductLotById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _productLotService.GetProductLotById(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }

        [HttpPost]

        public async Task<IActionResult> CreateProductLot([FromBody] List<ProductLotRequest> productLots)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _productLotService.CreateProductLot(productLots);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductLot([FromBody] ProductLotRequest productLot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _productLotService.UpdateProductLot(productLot);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }

        [HttpGet("CheckProductQuantity/{productId}")]
        public async Task<IActionResult> CheckProductQuantity(int productId)
        {
            var response = await _productLotService.CheckQuantityProduct(productId);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return Ok(new { response.Message, Quantity = response.Data });
        }


        [HttpPut("CheckAndUpdateExpiredLots")]
public async Task<IActionResult> CheckAndUpdateExpiredLots()
        {
            var response = await _productLotService.AutoUpdateProductLotStatusAsync();

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }

            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }


        [HttpGet("TotalQuantity/{productId}")]
        public async Task<IActionResult> GetTotalAvailableQuantity(int productId)
        {
            var response = await _productLotService.GetQuantityByProductIdAsync(productId);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors });
            }

            return Ok(new { response.Message, Quantity = response.Data });
        }

        [HttpGet("{productLotId}/compatible-storage-rooms")]
     
        public async Task<IActionResult> GetCompatibleStorageRooms(int productLotId)
        {
            var response = await _productLotService.ListCompatibleStorageRoomsAsync(productLotId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetProductLotsByStatus(int status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var response = await _productLotService.GetProductLotsByStatusAsync(status);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }

            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }
    }
}
