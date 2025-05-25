using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class ImageProduct
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? Image { get; set; }

        public virtual Product? Product { get; set; }
    }
}
