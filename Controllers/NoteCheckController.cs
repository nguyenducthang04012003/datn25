using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.NoteChecks;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteCheckController : ControllerBase
    {
        private readonly INoteCheckService _noteCheckService;

        public NoteCheckController(INoteCheckService noteCheckService)
        {
            _noteCheckService = noteCheckService;
        }

        [HttpPost("create")]
   
        public async Task<IActionResult> CreateNoteCheck([FromBody] NoteCheckRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var result = await _noteCheckService.CreateNoteCheckAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }





 

        
       

        [HttpGet("all-error-products")]
        public async Task<IActionResult> GetAllErrorProducts()
        {
            try
            {
                var result = await _noteCheckService.GetAllErrorProductsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        [HttpPut("cancel-error-product/{noteCheckDetailId}")]
        public async Task<IActionResult> CancelErrorProduct(int noteCheckDetailId)
        {
            try
            {
                var result = await _noteCheckService.UpdateErrorProductCancelStatusAsync(noteCheckDetailId);
                return Ok(new { Success = true, Message = "Đã hủy sản phẩm lỗi thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
    }

            [HttpGet]
        public async Task<IActionResult> GetAllNoteChecks()
        {
            try
            {
                var result = await _noteCheckService.GetAllNoteChecksAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }


        // Chi tiết của đơn kiểm kê
        [HttpGet("{noteCheckId}")]
        public async Task<IActionResult> GetNoteCheckById(int noteCheckId)
        {
            try
            {
                var result = await _noteCheckService.GetNoteCheckByIdAsync(noteCheckId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        // Duyệt đơn kiểm kê
        [HttpPut("{noteCheckId}/approve")]
        public async Task<IActionResult> ApproveNoteCheck(int noteCheckId)
        {
            try
            {
                var result = await _noteCheckService.ConfirmNoteCheckAsync(noteCheckId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpPut("update/{noteCheckId}")]
        public async Task<IActionResult> UpdateNoteCheck(int noteCheckId, [FromBody] NoteCheckRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var result = await _noteCheckService.UpdateNoteCheckAsync(noteCheckId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

    }






}
