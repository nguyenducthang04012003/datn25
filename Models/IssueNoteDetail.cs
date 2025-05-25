using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class IssueNoteDetail
    {
        public int IssueNoteDetailId { get; set; }
        public int? IssueNoteId { get; set; }
        public int? ProductLotId { get; set; }
        public int? Quantity { get; set; }

        public virtual IssueNote? IssueNote { get; set; }
        public virtual ProductLot? ProductLot { get; set; }
    }
}
