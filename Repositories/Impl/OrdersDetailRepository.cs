using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class OrdersDetailRepository : RepositoryBase<OrdersDetail>, IOrdersDetailRepository
    {
        public OrdersDetailRepository(SEP490_G74Context context) : base(context)
        {
            
        }

        public async Task AddOrdersDetails(List<OrdersDetail> listOrdersDetails)
        {
            await _context.OrdersDetails.AddRangeAsync(listOrdersDetails);
        }

        public async Task<List<int>> GetProductIdByOrderId(int orderId)
        {
            return await _context.OrdersDetails
                .Where(x => x.OrderId == orderId)
                .Select(x => x.ProductId)
                .Where(x => x.HasValue) // Loại bỏ null
                .Select(x => x.Value)   // Chuyển Nullable<int> -> int
                .ToListAsync();
        }
    }
}
