using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IPurchaseOrdersDetailRepository : IRepository<PurchaseOrdersDetail>
    {
        Task InsertRangeAsync(List<PurchaseOrdersDetail> purchaseOrdersDetails);
    }
}
