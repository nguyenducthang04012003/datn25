using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class IssueNoteRepository : RepositoryBase<IssueNote> , IIssueNoteRepository
    {
        public IssueNoteRepository(SEP490_G74Context context) : base(context)
        {
        }
        public async Task CreateIssueNote(IssueNote issueNote)
        {
            // Lấy ngày từ CreatedDate (nếu null thì lấy ngày hiện tại)
            var createdDate = issueNote.CreatedDate ?? DateTime.Now;

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
            issueNote.IssueNoteCode = $"{ConstantStringHelper.IssueNoteCode}{createdDate:ddMMyyyy}{nextOrderNumber}";

            _context.IssueNotes.Add(issueNote);
            int rowAffected = _context.SaveChanges();

        }
        public async Task<int> GetMaxOrderNumberByDate(DateTime createdDate)
        {
            // Tạo prefix chính xác dựa trên ngày tạo
            var orderCodePattern = $"{ConstantStringHelper.IssueNoteCode}{createdDate:ddMMyyyy}";

            // Lọc các đơn hàng theo ngày tháng năm, bỏ qua giờ phút giây
            var latestOrder = await _context.IssueNotes
                .Where(o => o.CreatedDate.HasValue &&
                            o.CreatedDate.Value.Year == createdDate.Year &&
                            o.CreatedDate.Value.Month == createdDate.Month &&
                            o.CreatedDate.Value.Day == createdDate.Day &&
                            o.IssueNoteCode.StartsWith(orderCodePattern)) // Chỉ lấy các OrderCode hợp lệ
                .OrderByDescending(o => o.IssueNoteCode)
                .FirstOrDefaultAsync();

            // Nếu không có đơn hàng nào trong ngày, bắt đầu từ 0
            if (latestOrder == null)
                return 0;

            // Lấy phần số thứ tự của OrderCode
            var lastOrderNumberStr = latestOrder.IssueNoteCode.Substring(orderCodePattern.Length);

            // Kiểm tra nếu không thể chuyển đổi thành số thì reset về 0
            return int.TryParse(lastOrderNumberStr, out int orderNumber) ? orderNumber : 0;
        }
    }
}
