using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class PurchaseOrdersDetailRepository :RepositoryBase<PurchaseOrdersDetail> , IPurchaseOrdersDetailRepository
    {
        public PurchaseOrdersDetailRepository(SEP490_G74Context context) : base(context)
        {
        }
        public async Task InsertRangeAsync(List<PurchaseOrdersDetail> purchaseOrdersDetails)
        {
            await _context.PurchaseOrdersDetails.AddRangeAsync(purchaseOrdersDetails);
        }
    }
}
