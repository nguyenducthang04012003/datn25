using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Models;
namespace PharmaDistiPro.Repositories.Interface
{
    public interface IReceivedNoteRepository : IRepository<ReceivedNote>
    {
        Task<List<ReceivedNote>> GetReceivedNoteList();
        Task<ReceivedNote> GetReceivedNoteById(int? id);

        Task<ReceivedNote> CreateReceivedNote(ReceivedNote ReceiveNote);
        Task<ReceivedNote> UpdateReceivedNote(ReceivedNote ReceiveNote);
        Task<ReceivedNoteDetail> CreateReceivedNoteDetail(ReceivedNoteDetail ReceiveNoteDetail);

        Task<List<ReceivedNoteDetail>> GetReceivedNoteDetailsByPurchaseOrderId(int? id);
        Task<List<ReceivedNoteDetail>> GetReceivedNoteDetailByReceivedNoteId(int? id);
        Task<ProductLot> GetProductLotById(int? id);
        Task<ProductLot> UpdateProductLot(ProductLot ProductLot);

    }
}
