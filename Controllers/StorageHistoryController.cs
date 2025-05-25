using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.Services.Interface;
using Microsoft.Extensions.Logging;
using PharmaDistiPro.Services.Impl;

namespace PharmaDistiPro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageHistoryController : ControllerBase
    {
        private readonly IStorageHistoryService _storageHistoryService;
        private readonly ILogger<StorageHistoryController> _logger;

        public StorageHistoryController(
            IStorageHistoryService storageHistoryService,
            ILogger<StorageHistoryController> logger)
        {
            _storageHistoryService = storageHistoryService ?? throw new ArgumentNullException(nameof(storageHistoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateStorageHistory([FromBody] StorageHistoryInputRequest request)
        {
           
            if (request == null)
            {
             
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                
                var result = await _storageHistoryService.CreateStorageHistoryAsync(request);
                _logger.LogInformation("Successfully created StorageHistory with ID {Id}", result.Id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error in CreateStorageHistory");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal error in CreateStorageHistory");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Top50Earliest/{storageRoomId}")]
        public async Task<IActionResult> GetTop50EarliestForChart(int storageRoomId)
        {
            try
            {
                var result = await _storageHistoryService.GetTop50EarliestForChartAsync(storageRoomId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching top 50 chart data for StorageRoomId {0}", storageRoomId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Newest/{storageRoomId}")]
        public async Task<IActionResult> GetLatestForRoom(int storageRoomId)
        {
            try
            {
                var result = await _storageHistoryService.GetLatestByStorageRoomIdAsync(storageRoomId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching latest data for StorageRoomId {0}", storageRoomId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }



        }
        [HttpGet("HasSensor/{storageRoomId}")]
        public async Task<IActionResult> HasSensor(int storageRoomId)
        {
            try
            {
                var hasSensor = await _storageHistoryService.HasSensorAsync(storageRoomId);
                return Ok(hasSensor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking sensor status for StorageRoomId {0}", storageRoomId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}