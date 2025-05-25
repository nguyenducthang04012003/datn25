using PharmaDistiPro.DTO.ProductShortage;
using PharmaDistiPro.DTO.PurchaseOrders;
using PharmaDistiPro.DTO.PurchaseOrdersDetails;

namespace PharmaDistiPro.Services.Interface
{
    public interface IPurchaseOrderService
    {
        #region purchase order
        Task<Response<PurchaseOrdersDto>> CreatePurchaseOrder(PurchaseOrdersRequestDto purchaseOrdersRequestDto);

        Task<Response<PurchaseOrdersDto>> UpdatePurchaseOrderStatus(int poId, int status);

        Task<Response<IEnumerable<PurchaseOrdersDto>>> GetPurchaseOrdersList(int[] status, DateTime? dateFrom, DateTime? dateTo);

        Task<Response<IEnumerable<PurchaseOrdersDto>>> GetPurchaseOrdersRevenueList(DateTime? dateFrom, DateTime? dateTo);
        #endregion

        Task<Response<IEnumerable<PurchaseOrdersDto>>> GetTopSupplierList(int? topSupplier);

        #region purchase order detail
        Task<Response<IEnumerable<PurchaseOrdersDetailDto>>> GetPurchaseOrderDetailByPoId(int poId);
        #endregion

        #region check po status
        Task<Response<List<ProductShortage>>> CheckReceivedStockStatus(int purchaseOrderId);
        #endregion
    }
}
