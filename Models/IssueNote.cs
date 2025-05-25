using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class IssueNote
    {
        public IssueNote()
        {
            IssueNoteDetails = new HashSet<IssueNoteDetail>();
        }

        public int IssueNoteId { get; set; }
        public string? IssueNoteCode { get; set; }
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? UpdatedStatusDate { get; set; }
        public double? TotalAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual User? Customer { get; set; }
        public virtual Order? Order { get; set; }
        public virtual ICollection<IssueNoteDetail> IssueNoteDetails { get; set; }
    }
}
