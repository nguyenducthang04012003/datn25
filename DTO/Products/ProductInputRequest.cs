using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
using System.ComponentModel.DataAnnotations;

namespace PharmaDistiPro.DTO.Products
{
    public class ProductInputRequest
    {
        public int? ProductId { get; set; }

     
        public string? ProductCode { get; set; }

    
        public string? ManufactureName { get; set; }


        public string? ProductName { get; set; }

       
        public string? Unit { get; set; }

       
        public int? CategoryId { get; set; }

     
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0.")]
        public double? SellingPrice { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool? Status { get; set; }

        [Range(0, 100, ErrorMessage = "VAT phải nằm trong khoảng từ 0 đến 100.")]
        public double? Vat { get; set; }

 
        public StorageCondition? Storageconditions { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Khối lượng phải lớn hơn 0.")]
        public double? Weight { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Thể tích phải lớn hơn 0.")]
        public double? VolumePerUnit { get; set; }

        [MaxLength(5, ErrorMessage = "Tối đa 5 ảnh.")]
        public List<IFormFile>? Images { get; set; }


    }
}
