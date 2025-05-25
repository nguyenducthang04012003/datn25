namespace PharmaDistiPro.DTO.ProductLots
{


    public class ProductLotRequest
    {
        public int ProductLotId { get; set; }
        public int LotId { get; set; }
        public int ProductId { get; set; }
        public DateTime ManufacturedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public int? OrderQuantity { get; set; }
        public double? SupplyPrice { get; set; }
        public int? Status { get; set; }
        public int? StorageRoomId { get; set; }
    }
}
