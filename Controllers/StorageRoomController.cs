using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.Models;
using PharmaDistiPro.Services.Impl;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageRoomController : ControllerBase
    {
        private readonly IStorageRoomService _storageRoomService;
        private readonly SEP490_G74Context _context;


        public StorageRoomController(IStorageRoomService storageRoomService, SEP490_G74Context context)
        {
            _storageRoomService = storageRoomService;
            _context = context;
        }


        //get storageRoom list
        [HttpGet("GetStorageRoomList")]
        public async Task<IActionResult> GetStorageRoomList()
        {
            var response = await _storageRoomService.GetStorageRoomList();
            if (!response.Success)
            {
                return Conflict(new { response.Message });
            }
            return Ok(response);
        }

        [HttpGet("RoomTypes")]
        public async Task<IActionResult> GetAllRoomTypes()
        {
            var types = await _storageRoomService.GetAllRoomTypes();
            return Ok(types);
        }

        // API  StorageRoom theo Id
        [HttpGet("GetStorageRoomById/{storageRoomId}")]
        public async Task<IActionResult> GetStorageRoomById(int storageRoomId)
        {
            var response = await _storageRoomService.GetStorageRoomById(storageRoomId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }


        // Api create storageRoom
        [HttpPost("CreateStorageRoom")]
        public async Task<IActionResult> CreateStorageRoom([FromForm] StorageRoomInputRequest storageRoom)
        {
            var response = await _storageRoomService.CreateNewStorageRoom(storageRoom);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!response.Success)
                return BadRequest(new { response.Message });

            return Ok(response);
        }

     

        [HttpPut("UpdateStorageRoom")]
        public async Task<IActionResult> UpdateStorageRoom([FromBody] StorageRoomInputRequest storageRoom)
        {
            try
            {
               
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();
                    return BadRequest(new { Message = "Dữ liệu không hợp lệ.", Errors = errors });
                }

                
                var response = await _storageRoomService.UpdateStorageRoom(storageRoom);

               
                if (!response.Success)
                    return BadRequest(new { response.Message });

               
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi API UpdateStorageRoom: {ex.Message}");
                return StatusCode(500, new { Message = "Lỗi máy chủ nội bộ." });
            }
        }
        [HttpPut("ActivateDeactivateStorageRoom/{storageRoomId}/{status}")]
        public async Task<IActionResult> ActivateDeactivateStorageRoom(int storageRoomId, bool status)
        {
            var response = await _storageRoomService.ActivateDeactivateStorageRoom(storageRoomId, status);
            if (!response.Success) return BadRequest(new { response.Message });
            return Ok(response);
        }

      

    }
}
