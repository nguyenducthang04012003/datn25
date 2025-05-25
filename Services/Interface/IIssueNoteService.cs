using PharmaDistiPro.DTO.IssueNote;
using PharmaDistiPro.DTO.IssueNoteDetails;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Services.Interface
{
    public interface IIssueNoteService
    {
        #region issue note
        // create issue note
        public Task<Response<IssueNoteDto>> CreateIssueNote(int orderId);

        // cancel issue note
        public Task<Response<IssueNoteDto>> CancelIssueNote(int issueNoteId);

        // get issue note by warehouse id
        public Task<Response<IEnumerable<IssueNoteDto>>> GetIssueNoteByWarehouseId(int[]? status);

        // get issue note by issue note id
        public Task<Response<IEnumerable<IssueNoteDto>>> GetIssueNoteList();

        public Task<Response<IssueNoteDto>> UpdateIssueNoteStatus(int issueNoteId, int status);

        #endregion

        #region issue note detail
        //get issue note detail list
        public Task<Response<IEnumerable<IssueNoteDetailDto>>> GetIssueNoteDetailsList();

        //get issue note detail by issue note id
        public Task<Response<IEnumerable<IssueNoteDetailDto>>> GetIssueNoteDetailByIssueNoteId(int issueNoteId);
        #endregion
    }
}
