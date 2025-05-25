using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.PurchaseOrders
{
    public class PurchaseOrdersDto
    {
        public int PurchaseOrderId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? UpdatedStatusDate { get; set; }
        public double? TotalAmount { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }

        public double? AmountPaid { get; set; }

        public virtual UserDTO? CreatedByNavigation { get; set; }

        public virtual SupplierDTO? Supplier { get; set; }
    }
}
