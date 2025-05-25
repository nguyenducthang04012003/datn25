using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class Order
    {
        public Order()
        {
            IssueNotes = new HashSet<IssueNote>();
            OrdersDetails = new HashSet<OrdersDetail>();
        }

        public int OrderId { get; set; }
        public string? OrderCode { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? UpdatedStatusDate { get; set; }
        public DateTime? StockReleaseDate { get; set; }
        public double? TotalAmount { get; set; }
        public int? Status { get; set; }
        public string? WardCode { get; set; }
        public int? DistrictId { get; set; }
        public double? DeliveryFee { get; set; }
        public string? Address { get; set; }
        public int? ConfirmedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? AssignTo { get; set; }
        public bool? StatusPayment { get; set; }

        public virtual User? AssignToNavigation { get; set; }
        public virtual User? ConfirmedByNavigation { get; set; }
        public virtual User? Customer { get; set; }
        public virtual ICollection<IssueNote> IssueNotes { get; set; }
        public virtual ICollection<OrdersDetail> OrdersDetails { get; set; }
    }
}
