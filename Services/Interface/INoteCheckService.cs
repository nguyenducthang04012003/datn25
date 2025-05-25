using PharmaDistiPro.DTO.NoteCheckDetails;
using PharmaDistiPro.DTO.NoteChecks;

namespace PharmaDistiPro.Services.Interface
{
    public interface INoteCheckService
    {
     
        Task<NoteCheckDTO> ConfirmNoteCheckAsync(int noteCheckId);
        Task<NoteCheckDTO> CreateNoteCheckAsync(NoteCheckRequestDTO request);
        Task<List<ErrorProductDTO>> GetAllErrorProductsAsync();
        Task<List<NoteCheckDTO>> GetAllNoteChecksAsync();
        Task<NoteCheckDTO> GetNoteCheckByIdAsync(int noteCheckId);
        Task<bool> UpdateErrorProductCancelStatusAsync(int noteCheckDetailId);
        Task<NoteCheckDTO> UpdateNoteCheckAsync(int noteCheckId, NoteCheckRequestDTO request);
    }
}
