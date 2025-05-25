using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class NoteCheckDetail
    {
        public int NoteCheckDetailId { get; set; }
        public int? NoteCheckId { get; set; }
        public int? ProductLotId { get; set; }
        public int? StorageQuantity { get; set; }
        public int? DifferenceQuatity { get; set; }
        public int? ActualQuantity { get; set; }
        public int? ErrorQuantity { get; set; }
        public int? Status { get; set; }

        public virtual NoteCheck? NoteCheck { get; set; }
        public virtual ProductLot? ProductLot { get; set; }
    }
}
