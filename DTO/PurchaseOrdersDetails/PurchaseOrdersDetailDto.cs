using PharmaDistiPro.DTO.Products;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.PurchaseOrdersDetails
{
    public class PurchaseOrdersDetailDto
    {
        public int PurchaseOrderDetailId { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? SupplyPrice { get; set; }
        public virtual ProductOrderDto? Product { get; set; }
    }
}
