using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(SEP490_G74Context context) : base(context)
        {
        }

        public async Task<PurchaseOrder> GetPoById(int poId)
        {
           return await _context.PurchaseOrders
                .Include(po => po.PurchaseOrdersDetails)
                .ThenInclude(pod => pod.Product)
                .Include(po => po.ReceivedNotes)
                .ThenInclude(rn => rn.ReceivedNoteDetails)
                .ThenInclude(rnd => rnd.ProductLot) // Để lấy thông tin ProductId từ ProductLot
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == poId);
        }

        public async Task InsertPurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            // Lấy ngày từ CreatedDate (nếu null thì lấy ngày hiện tại)
            var createdDate = purchaseOrder.CreateDate ?? DateTime.Now;

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
            purchaseOrder.PurchaseOrderCode = $"{ConstantStringHelper.PurchaseOrderCode}{createdDate:ddMMyyyy}{nextOrderNumber}";

            // Chèn vào DB
            await _context.PurchaseOrders.AddAsync(purchaseOrder);
            await _context.SaveChangesAsync();
        }

        // Lấy số thứ tự lớn nhất trong ngày hiện tại
        public async Task<int> GetMaxOrderNumberByDate(DateTime createdDate)
        {
            // Tạo prefix chính xác dựa trên ngày tạo
            var orderCodePattern = $"{ConstantStringHelper.PurchaseOrderCode}{createdDate:ddMMyyyy}";

            // Tìm đơn hàng gần nhất theo ngày, bỏ qua giờ phút giây
            var latestOrder = await _context.PurchaseOrders
                .Where(o => o.CreateDate.HasValue &&
                            o.CreateDate.Value.Date == createdDate.Date && // So sánh chỉ ngày
                            o.PurchaseOrderCode.StartsWith(orderCodePattern)) // Chỉ lấy các mã đúng format
                .OrderByDescending(o => o.PurchaseOrderCode)
                .FirstOrDefaultAsync();

            // Nếu không có đơn hàng nào trong ngày, bắt đầu từ 0
            if (latestOrder == null)
                return 0;

            // Lấy phần số thứ tự của OrderCode
            var lastOrderNumberStr = latestOrder.PurchaseOrderCode.Substring(orderCodePattern.Length);

            // Nếu không thể chuyển đổi thành số thì reset về 0
            return int.TryParse(lastOrderNumberStr, out int orderNumber) ? orderNumber : 0;
        }
    }
}
