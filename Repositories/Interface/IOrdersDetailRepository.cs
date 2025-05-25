using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IOrdersDetailRepository : IRepository<OrdersDetail>
    {
        Task<List<int>> GetProductIdByOrderId(int orderId);

        Task AddOrdersDetails(List<OrdersDetail> listOrdersDetails);

    }
}
