using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.ReceivedNotes
{
    public class ReceivedNoteResponse
    {
        public int ReceiveNoteId { get; set; }
        public string ReceivedNoteCode { get; set; }
        public int PurchaseOrderId { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ReceiveNoteDetailResponse>? ReceivedNoteDetails { get; set; } = new List<ReceiveNoteDetailResponse>();

    }
    public class ReceiveNoteDetailResponse
    {
        public int ReceiveNoteDetailId { get; set; }
        public string? NoteNumber { get; set; }
        public int? ProductLotId { get; set; }
        public int? UnitPrice { get; set; }
        public int? ActualReceived { get; set; }
        public string? DocumentNumber { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Unit { get; set; }
        public string? LotCode { get; set; }
        public int? TotalAmount
        {
            get
            {
                return UnitPrice * ActualReceived;
            }
        }

    }
}
