namespace PharmaDistiPro.DTO.IssueNote
{
    public class IssueNoteDto
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

    }
}
