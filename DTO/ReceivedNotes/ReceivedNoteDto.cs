using PharmaDistiPro.DTO.PurchaseOrders;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.ReceivedNotes
{
    public class ReceivedNoteDto
    {
        public int ReceiveNoteId { get; set; }
        public string? ReceiveNotesCode { get; set; }
        public int? PurchaseOrderId { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual UserDTO? CreatedByNavigation { get; set; }
        public virtual PurchaseOrdersDto? PurchaseOrder { get; set; }

    }


    

}
