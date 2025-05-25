using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.Carts
{
    public class CartModelDto
    {
        public int ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public int ? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Vat { get; set; }
    }
}
