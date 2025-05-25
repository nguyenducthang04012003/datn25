using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.Orders
{
    public class OrderRequestDto
    {
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

        public virtual ICollection<OrdersDetailsRequestDto> OrdersDetails { get; set; }
    }

    public class OrdersDetailsRequestDto
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

    }
}
