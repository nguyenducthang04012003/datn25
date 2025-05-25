using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class ProductStorageRoom
    {
        public int Id { get; set; }
        public int? StorageRoomId { get; set; }
        public int? ProductLotId { get; set; }

        public virtual ProductLot? ProductLot { get; set; }
        public virtual StorageRoom? StorageRoom { get; set; }
    }
}
