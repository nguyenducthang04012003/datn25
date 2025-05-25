using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class PurchaseOrder
    {
        public PurchaseOrder()
        {
            PurchaseOrdersDetails = new HashSet<PurchaseOrdersDetail>();
            ReceivedNotes = new HashSet<ReceivedNote>();
        }

        public int PurchaseOrderId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? UpdatedStatusDate { get; set; }
        public double? TotalAmount { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<PurchaseOrdersDetail> PurchaseOrdersDetails { get; set; }
        public virtual ICollection<ReceivedNote> ReceivedNotes { get; set; }
    }
}
