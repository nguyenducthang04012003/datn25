using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class ReceiveNoteDetail
    {
        public int ReceiveNoteDetailId { get; set; }
        public string? NoteNumber { get; set; }
        public int? ProductLotId { get; set; }
        public int? UnitPrice { get; set; }
        public int? ActualReceived { get; set; }
        public string? DocumentNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual ProductLot? ProductLot { get; set; }
    }
}
