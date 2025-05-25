namespace PharmaDistiPro.DTO.ProductLots
{
    public class ProductLotResponse
    {
        public int Id { get; set; }
        public int? LotId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public double? SupplyPrice { get; set; }
             public int? OrderQuantity { get; set; }
        public int? Status { get; set; }

        public string ProductName { get; set; }
        = string.Empty;
        public string LotCode { get; set; } = string.Empty;
        public int? StorageRoomId { get; set; }
    }
}
