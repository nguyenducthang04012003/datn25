using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class Manufacture
    {
        public Manufacture()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? ManufacturesCode { get; set; }
        public string? ManufacturesName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
