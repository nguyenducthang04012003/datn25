using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueNoteController : ControllerBase
    {
        private readonly IIssueNoteService  _issueNoteService;
        public IssueNoteController(IIssueNoteService issueNoteService)
        {
            _issueNoteService = issueNoteService;
        }

        #region issue note

        // api create issue note
        [HttpPost("CreateIssueNote/{orderId}")]
        public async Task<IActionResult> CreateIssueNote(int orderId)
        {
            var response = await _issueNoteService.CreateIssueNote(orderId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api cancel issue note
        [HttpPut("CancelIssuteNot/{issueNoteId}")]
        public async Task<IActionResult> CancelIssueNote(int issueNoteId)
        {
            var response = await _issueNoteService.CancelIssueNote(issueNoteId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api get issue note list
        [HttpGet("GetIssueNoteList")]
        public async Task<IActionResult> GetIssueNoteList()
        {
            var response = await _issueNoteService.GetIssueNoteList();
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api get issue note by warehouse id
        [HttpGet("GetIssueNoteListByWarehouse")]
        public async Task<IActionResult> GetIssueNoteListByWarehouse([FromQuery] int[]? status)
        {
            var response = await _issueNoteService.GetIssueNoteByWarehouseId(status);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api update issue note status
        [HttpPut("UpdateIssueNoteStatus/{issueNoteId}/{status}")]
        public async Task<IActionResult> UpdateIssueNoteStatus(int issueNoteId, int status)
        {
            var response = await _issueNoteService.UpdateIssueNoteStatus(issueNoteId, status);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        #endregion

        #region issue note details
        // api get issue note detail by issue note id
        [HttpGet("GetIssueNoteDetailByIssueNoteId/{issueNoteId}")]
        public async Task<IActionResult> GetIssueNoteDetailByIssueNoteId(int issueNoteId)
        {
            var response = await _issueNoteService.GetIssueNoteDetailByIssueNoteId(issueNoteId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api get issue note detail list
        [HttpGet("GetIssueNoteDetailsList")]
        public async Task<IActionResult> GetIssueNoteDetailsList()
        {
            var response = await _issueNoteService.GetIssueNoteDetailsList();
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }
        #endregion

    }
}
