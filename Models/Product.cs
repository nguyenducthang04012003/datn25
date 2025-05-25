using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class Product
    {
        public Product()
        {
            ImageProducts = new HashSet<ImageProduct>();
            OrdersDetails = new HashSet<OrdersDetail>();
            ProductLots = new HashSet<ProductLot>();
            PurchaseOrdersDetails = new HashSet<PurchaseOrdersDetail>();
        }

        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ManufactureName { get; set; }
        public string? ProductName { get; set; }
        public string? Unit { get; set; }
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
        public double? SellingPrice { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }
        public double? Vat { get; set; }
        public int? Storageconditions { get; set; }
        public double? Weight { get; set; }
        public double? VolumePerUnit { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<ImageProduct> ImageProducts { get; set; }
        public virtual ICollection<OrdersDetail> OrdersDetails { get; set; }
        public virtual ICollection<ProductLot> ProductLots { get; set; }
        public virtual ICollection<PurchaseOrdersDetail> PurchaseOrdersDetails { get; set; }
    }
}
