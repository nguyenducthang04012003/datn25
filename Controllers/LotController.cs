using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.Lots;
using PharmaDistiPro.Models;
using PharmaDistiPro.Services.Impl;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotController : ControllerBase
    {
        private readonly ILotService _lotService;

        public LotController(ILotService lotService)
        {
            _lotService = lotService;
        }

        // GET: api/Lot
        [HttpGet]
        public async Task<ActionResult<List<LotResponse>>> GetLots()
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var response = await _lotService.GetLotList();

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }

        // GET: api/Lot/{lotCode}
        [HttpGet("{lotCode}")]

        public async Task<IActionResult> GetLotByLotCode(string lotCode)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var response = await _lotService.GetLotByLotCode(lotCode);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            
        }

        // POST: api/Lot
        [HttpPost]
        public async Task<ActionResult<LotResponse>> CreateLot([FromBody] LotRequest lot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var response = await _lotService.CreateLot(lot);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            
        }

        [HttpPut("{lotCode}")]
        public async Task<ActionResult<LotResponse>> UpdateLot(string lotCode,[FromBody] LotRequest lot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            var response =  await _lotService.UpdateLot(lotCode,lot);

            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            
        }

    }
}
