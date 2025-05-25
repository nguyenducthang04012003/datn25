using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.Models;
using PharmaDistiPro.Services.Interface;
using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.Services.Impl;
using Microsoft.AspNetCore.Authorization;
namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly SEP490_G74Context _context;
        public SupplierController(ISupplierService supplierService, SEP490_G74Context context)
        {
            _supplierService = supplierService;
            _context = context;
        }

        //get supplier list
        [HttpGet("GetSupplierList")]
    
        public async Task<IActionResult> GetSupplierList()
        {
            var response = await _supplierService.GetSupplierList();
            if (!response.Success)
            {
                return Conflict(new { response.Message });
            }
            return Ok(response);
        }



        // API lấy thông tin Supplier theo Id
        [HttpGet("GetSupplierById/{supplierId}")]
   
        public async Task<IActionResult> GetSupplierById(int supplierId)
        {
            var response = await _supplierService.GetSupplierById(supplierId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }


        // Api create supplier
        [HttpPost("CreateSupplier")]
 
        public async Task<IActionResult> CreateSupplier([FromBody] SupplierInputRequest supplier)
        {
            var response = await _supplierService.CreateNewSupplier(supplier);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response);
        }

        //Api update supplier
        [HttpPut("UpdateSupplier")]
      
        public async Task<IActionResult> UpdateSupplier([FromForm] SupplierInputRequest supplier)
        {
            var response = await _supplierService.UpdateSupplier(supplier);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response);
        }

        [HttpPut("ActivateDeactivateSupplier/{supplierId}/{status}")]
   
        public async Task<IActionResult> ActivateDeactivateSupplier(int supplierId, bool status)
        {
            var response = await _supplierService.ActivateDeactivateSupplier(supplierId, status);
            if (!response.Success) return BadRequest(new { response.Message });
            return Ok(response);
        }
    }
}
