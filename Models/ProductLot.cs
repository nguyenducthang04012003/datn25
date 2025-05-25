using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class ProductLot
    {
        public ProductLot()
        {
            IssueNoteDetails = new HashSet<IssueNoteDetail>();
            NoteCheckDetails = new HashSet<NoteCheckDetail>();
            ReceivedNoteDetails = new HashSet<ReceivedNoteDetail>();
        }

        public int ProductLotId { get; set; }
        public int? ProductId { get; set; }
        public int? LotId { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public double? SupplyPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public int? Quantity { get; set; }
        public int? Status { get; set; }
        public int? StorageRoomId { get; set; }

        public virtual Lot? Lot { get; set; }
        public virtual Product? Product { get; set; }
        public virtual StorageRoom? StorageRoom { get; set; }
        public virtual ICollection<IssueNoteDetail> IssueNoteDetails { get; set; }
        public virtual ICollection<NoteCheckDetail> NoteCheckDetails { get; set; }
        public virtual ICollection<ReceivedNoteDetail> ReceivedNoteDetails { get; set; }
    }
}
