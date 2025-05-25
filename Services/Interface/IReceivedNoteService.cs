using PharmaDistiPro.DTO.ReceivedNotes;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Services.Interface
{
    public interface IReceivedNoteService
    {
        Task<Services.Response<List<ReceivedNoteDto>>> GetReceiveNoteList();
        Task<Services.Response<ReceivedNoteResponse>> GetReceiveNoteById(int id);

        Task<Services.Response<ReceivedNoteDto>> CreateReceiveNote(ReceivedNoteRequest ReceiveNote);
        Task<Services.Response<ReceivedNoteDto>> CancelReceiveNote(int? ReceivedNoteId);

    }
}
