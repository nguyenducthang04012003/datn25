using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.DTO.Units;
using PharmaDistiPro.Models;
using PharmaDistiPro.Services.Impl;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {

        private readonly IUnitService _unitService;
        private readonly SEP490_G74Context _context;
        public UnitsController(IUnitService unitService, SEP490_G74Context context)
        {
            _unitService =unitService;
            _context = context;
        }

        //get unit list
        [HttpGet("GetUnitist")]
        public async Task<IActionResult> GetUnitList()
        {
            var response = await _unitService.GetUnitList();
            if (!response.Success)
            {
                return Conflict(new { response.Message });
            }
            return Ok(response);
        }



        // API unit by Id
        [HttpGet("GetUnitById/{unitId}")]
        public async Task<IActionResult> GetUnitById(int unitId)
        {
            var response = await _unitService.GetUnitById(unitId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }


        // Api create unit
        [HttpPost("CreateUnit")]
        public async Task<IActionResult> CreateUnit([FromForm]UnitInputRequest unit)
        {
            var response = await _unitService.CreateNewUnits(unit);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response);
        }

        //Api update unit
        [HttpPut("UpdateUnit")]
        public async Task<IActionResult> UpdateUnit([FromForm] UnitInputRequest unit)
        {
            var response = await _unitService.UpdateUnit(unit);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response);
        }
    }
}
