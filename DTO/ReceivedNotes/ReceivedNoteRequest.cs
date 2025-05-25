using PharmaDistiPro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PharmaDistiPro.DTO.ReceivedNotes
{
    public class ReceivedNoteRequest
    {
        public int? PurchaseOrderId { get; set; }

        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public List<ReceivedNoteDetailRequest> ReceivedNoteDetail { get; set; } = new List<ReceivedNoteDetailRequest>();

    }

    public class ReceivedNoteDetailRequest
    {
        public int ReceiveNoteId { get; set; }
        public int ProductLotId { get; set; }
        public int ActualReceived { get; set; } 

    }
}
