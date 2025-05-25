namespace PharmaDistiPro.DTO.Products
{
    public class ProductOrderDto
    {
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
    }
}
