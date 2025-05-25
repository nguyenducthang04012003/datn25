using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class StorageRoom
    {
        public StorageRoom()
        {
            NoteChecks = new HashSet<NoteCheck>();
            ProductLots = new HashSet<ProductLot>();
            StorageHistories = new HashSet<StorageHistory>();
        }

        public int StorageRoomId { get; set; }
        public string? StorageRoomCode { get; set; }
        public string? StorageRoomName { get; set; }
        public int? Type { get; set; }
        public double? Capacity { get; set; }
        public double? RemainingRoomVolume { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<NoteCheck> NoteChecks { get; set; }
        public virtual ICollection<ProductLot> ProductLots { get; set; }
        public virtual ICollection<StorageHistory> StorageHistories { get; set; }
    }
}
