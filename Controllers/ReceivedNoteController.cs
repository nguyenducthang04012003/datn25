using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.ReceivedNotes;
using PharmaDistiPro.Services.Interface;
using PharmaDistiPro.Models;
using Microsoft.AspNetCore.Authorization;
namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceivedNoteController : ControllerBase
    {
        private readonly IReceivedNoteService _receivedNoteService;

        public ReceivedNoteController(IReceivedNoteService receivedNoteService)
        {
            _receivedNoteService = receivedNoteService;
        }


        [HttpGet]
        public async Task<ActionResult<List<ReceivedNoteResponse>>> GetReceivedNoteList()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _receivedNoteService.GetReceiveNoteList();
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceivedNoteById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _receivedNoteService.GetReceiveNoteById(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }
        [HttpPost]

        public async Task<IActionResult> CreateReceivedNote([FromBody] ReceivedNoteRequest receivedNote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _receivedNoteService.CreateReceiveNote(receivedNote);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }
        [HttpPut]
        public async Task<IActionResult> CancelReceivedNote([FromBody] int receivedNoteId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid model state", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
            var response = await _receivedNoteService.CancelReceiveNote(receivedNoteId);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
            }
            return StatusCode(response.StatusCode, new { response.Message, response.Errors, response.Data });
        }
    }
}
