namespace PharmaDistiPro.DTO.Suppliers
{
    public class SupplierInputRequest
    {
        public int? Id { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhone { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
