using PharmaDistiPro.DTO.Products;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.OrdersDetails
{
    public class OrdersDetailDto
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }

        public int? TotalQuantity { get; set; }
        public virtual ProductOrderDto? Product { get; set; }
    }
}
