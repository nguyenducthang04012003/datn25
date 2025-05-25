using PharmaDistiPro.DTO.IssueNoteDetails;
using PharmaDistiPro.DTO.Products;

namespace PharmaDistiPro.DTO.NoteCheckDetails
{
    public class NoteCheckDetailsDTO
    {
        public int NoteCheckDetailId { get; set; }
        public int? NoteCheckId { get; set; }
        public int? ProductLotId { get; set; }
        public int? StorageQuantity { get; set; }
        public int? DifferenceQuatity { get; set; }
        public int? ActualQuantity { get; set; }
        public int? ErrorQuantity { get; set; }
        public int? Status { get; set; }


        public virtual ProductLotCheckNoteDetailsDTO? ProductLot { get; set; }
    
    
    }

        public class ProductLotCheckNoteDetailsDTO
        {
            public int ProductLotId { get; set; }
            public int? ProductId { get; set; }
            public int? LotId { get; set; }
            public DateTime? ManufacturedDate { get; set; }
            public DateTime? ExpiredDate { get; set; }
            public double? SupplyPrice { get; set; }
            public int? Quantity { get; set; }
            public int? Status { get; set; }
            public int? StorageRoomId { get; set; }
            public virtual ProductDTO? Product { get; set; }
        }
    
}
