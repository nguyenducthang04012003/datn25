using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface INoteCheckRepository : IRepository<NoteCheck>
    {
        Task<List<NoteCheck>> GetAllWithDetailsAsync();
        Task<NoteCheck> GetByCodeWithDetailsAsync(string noteCheckCode);
        Task<NoteCheckDetail> GetDetailByIdAsync(int noteCheckDetailId);
        Task<List<NoteCheckDetail>> GetDetailsByNoteCheckIdAsync(int noteCheckId);
        Task<NoteCheck> GetNoteCheckByIdAsync(int noteCheckId);
        Task InsertNoteCheckAsync(NoteCheck notecheck);
        Task UpdateDetailAsync(NoteCheckDetail noteCheckDetail);
        Task UpdateNoteCheckAsync(NoteCheck noteCheck);
    }
}
