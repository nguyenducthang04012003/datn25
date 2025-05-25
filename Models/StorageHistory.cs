using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class StorageHistory
    {
        public int Id { get; set; }
        public int? StorageRoomId { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Service { get; set; }

        public virtual StorageRoom? StorageRoom { get; set; }
    }
}
