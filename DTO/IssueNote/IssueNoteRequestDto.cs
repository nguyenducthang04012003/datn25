     namespace PharmaDistiPro.DTO.IssueNote
{
    public class IssueNoteRequestDto
    {
        public int IssueNoteId { get; set; }
        public string? IssueNoteCode { get; set; }
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? UpdatedStatusDate { get; set; }
        public double? TotalAmount { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }


        public virtual ICollection<IssueNoteDetailRequestDto> IssueNoteDetails { get; set; } = new List<IssueNoteDetailRequestDto>();
    }

    public class IssueNoteDetailRequestDto
    {
        public int NoteCheckDetailId { get; set; }
        public int? NoteCheckId { get; set; }
        public int? ProductLotId { get; set; }
        public int? StorageQuantity { get; set; }
        public int? ActualQuantity { get; set; }
        public int? ErrorQuantity { get; set; }
        public int? Status { get; set; }
    }
}
