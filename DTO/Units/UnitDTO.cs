using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.Units
{
    public class UnitDTO
    {
        public int Id { get; set; }
        public string? UnitsName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
