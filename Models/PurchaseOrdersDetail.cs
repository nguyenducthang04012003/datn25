using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class PurchaseOrdersDetail
    {
        public int PurchaseOrderDetailId { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? SupplyPrice { get; set; }

        public virtual Product? Product { get; set; }
        public virtual PurchaseOrder? PurchaseOrder { get; set; }
    }
}
