using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Helper;

namespace PharmaDistiPro.Repositories.Impl
{
    public class ReceivedNoteRepository : RepositoryBase<ReceivedNote>, IReceivedNoteRepository
    {
        public ReceivedNoteRepository(SEP490_G74Context context) : base(context)
        {
        }

        public async Task<ReceivedNote> CreateReceivedNote(ReceivedNote ReceiveNote)
        {
            // Lấy ngày từ CreatedDate (nếu null thì lấy ngày hiện tại)
            var createdDate = ReceiveNote.CreatedDate ?? DateTime.Now;

            // Lấy số thứ tự lớn nhất trong ngày hiện tại
            var maxOrderNumber = await GetMaxOrderNumberByDate(createdDate);

            // Tạo số thứ tự tiếp theo
            var nextOrderNumber = (maxOrderNumber + 1).ToString();

            // Nếu số thứ tự nhỏ hơn 100, đảm bảo có ít nhất 3 chữ số (001 -> 099, 100 giữ nguyên)
            if (maxOrderNumber + 1 < 100)
            {
                nextOrderNumber = nextOrderNumber.PadLeft(3, '0');
            }

            // Tạo mã đơn hàng theo format mong muốn
            ReceiveNote.ReceiveNotesCode = $"{ConstantStringHelper.ReceviedCode}{createdDate:ddMMyyyy}{nextOrderNumber}";

            _context.ReceivedNotes.Add(ReceiveNote);
            int rowAffected = _context.SaveChanges();
            return await GetReceivedNoteById(ReceiveNote.ReceiveNoteId);
        }
        public async Task<int> GetMaxOrderNumberByDate(DateTime createdDate)
        {
            // Tạo prefix chính xác dựa trên ngày tạo
            var orderCodePattern = $"{ConstantStringHelper.ReceviedCode}{createdDate:ddMMyyyy}";

            // Lọc các đơn hàng theo ngày tháng năm, bỏ qua giờ phút giây
            var latestOrder = await _context.ReceivedNotes
                .Where(o => o.CreatedDate.HasValue &&
                            o.CreatedDate.Value.Year == createdDate.Year &&
                            o.CreatedDate.Value.Month == createdDate.Month &&
                            o.CreatedDate.Value.Day == createdDate.Day &&
                            o.ReceiveNotesCode.StartsWith(orderCodePattern)) // Chỉ lấy các OrderCode hợp lệ
                .OrderByDescending(o => o.ReceiveNotesCode)
                .FirstOrDefaultAsync();

            // Nếu không có đơn hàng nào trong ngày, bắt đầu từ 0
            if (latestOrder == null)
                return 0;

            // Lấy phần số thứ tự của OrderCode
            var lastOrderNumberStr = latestOrder.ReceiveNotesCode.Substring(orderCodePattern.Length);

            // Kiểm tra nếu không thể chuyển đổi thành số thì reset về 0
            return int.TryParse(lastOrderNumberStr, out int orderNumber) ? orderNumber : 0;
        }

        public async Task<ReceivedNote> GetReceivedNoteById(int? id)
        {
            return await _context.ReceivedNotes.Include(r => r.PurchaseOrder).FirstOrDefaultAsync(x => x.ReceiveNoteId == id);
        }
        public async Task<List<ReceivedNoteDetail>> GetReceivedNoteDetailByReceivedNoteId(int? id)
        {
            return await _context.ReceivedNoteDetails                                 
                                 .Include(r => r.ProductLot)
                                 .ThenInclude(pl => pl.Product)
                                 .Include(r => r.ProductLot)
                                 .ThenInclude(pl => pl.Lot)
                                 .Where(r => r.ReceiveNoteId == id)
                                 .ToListAsync();
        }
        public Task<List<ReceivedNote>> GetReceivedNoteList()
        {
            return _context.ReceivedNotes.Include(x=> x.PurchaseOrder)
                .Include(x=> x.CreatedByNavigation)
                .ToListAsync();
        }

        public Task<ReceivedNote> UpdateReceivedNote(ReceivedNote ReceiveNote)
        {
            _context.ReceivedNotes.Update(ReceiveNote);
            int rowAffected = _context.SaveChanges();
            return GetReceivedNoteById(ReceiveNote.ReceiveNoteId);
        }

        public async Task<ReceivedNoteDetail> CreateReceivedNoteDetail(ReceivedNoteDetail ReceiveNoteDetail)
        {

            _context.ReceivedNoteDetails.Add(ReceiveNoteDetail);
            int rowAffected = await _context.SaveChangesAsync();
            return ReceiveNoteDetail;
        }

        public async Task<ProductLot> GetProductLotById(int? id)
        {
            return await _context.ProductLots.FirstOrDefaultAsync(x => x.ProductLotId == id);
        }

        public async Task<ProductLot> UpdateProductLot(ProductLot ProductLot)
        {
            _context.ProductLots.Update(ProductLot);
            int rowAffected = await _context.SaveChangesAsync();
            return ProductLot;
        }

        public Task<List<ReceivedNoteDetail>> GetReceivedNoteDetailsByPurchaseOrderId(int? id)
        {
            return _context.ReceivedNoteDetails.Include(x=> x.ProductLot).Where(x => x.ReceiveNoteId == id).ToListAsync();
        }
    }
    
}
