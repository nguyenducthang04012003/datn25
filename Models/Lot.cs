using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class Lot
    {
        public Lot()
        {
            ProductLots = new HashSet<ProductLot>();
        }

        public int LotId { get; set; }
        public string? LotCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<ProductLot> ProductLots { get; set; }
    }
}
