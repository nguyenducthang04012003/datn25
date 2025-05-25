using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IOrderRepository : IRepository<Order>
    {
         Task InsertOrderAsync(Order order);
    }
}
