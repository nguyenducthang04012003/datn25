using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.Suppliers
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhone { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

     
    }
}
