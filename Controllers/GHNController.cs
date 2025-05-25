using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.Services.Interface;
using PharmaDistiPro.DTO.GHN;
namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GHNController : ControllerBase
    {
        private readonly IGHNService _ghnService;

        public GHNController(IGHNService ghnService)
        {
            _ghnService = ghnService;
        }
        #region address
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _ghnService.GetProvinces();
            return StatusCode(response.StatusCode, new { response.Message, response.Data });
        }

        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _ghnService.GetDistricts(provinceId);
            return StatusCode(response.StatusCode, new { response.Message, response.Data });
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWards(int districtId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _ghnService.GetWards(districtId);
            return StatusCode(response.StatusCode, new { response.Message, response.Data });
        }
        #endregion

        [HttpPost("create-orderGHN/{orderId}")]
        public async Task<IActionResult> CreateOrder(int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var result = await _ghnService.CreateOrder(orderId);

                return StatusCode(result.StatusCode, result);
        }

        [HttpPost("calculate-fee")]
        public async Task<IActionResult> CalculateOrderFee(FeeRequest orderRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var result = await _ghnService.CalculateShippingFee(orderRequest);

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("get-services-type")]
        public async Task<IActionResult> GetServiceTypes(int fromDistrictId, int toDistrictId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var result = await _ghnService.GetServiceTypes(fromDistrictId,toDistrictId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("calculate-expected-delivery-time")]
        
        public async Task<IActionResult> GetExpectedDeliveryTime(int fromDistrictId, string fromWardCode, int toDistrictId, string toWardCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var result = await _ghnService.GetExpectedDateDelivery(fromDistrictId, fromWardCode, toDistrictId, toWardCode);
            return StatusCode(result.StatusCode, result);
        }
        

    }
}
