using PharmaDistiPro.DTO.Products;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.IssueNoteDetails
{
    public class IssueNoteDetailDto
    {
        public int IssueNoteDetailId { get; set; }
        public int? IssueNoteId { get; set; }
        public int? ProductLotId { get; set; }
        public int? Quantity { get; set; }
        public virtual ProductLotIssueNoteDetailsDto? ProductLot { get; set; }
    }



    public class ProductLotIssueNoteDetailsDto
    {
        public int ProductLotId { get; set; }
        public int? ProductId { get; set; }
        public int? LotId { get; set; }

        public int? StorageRoomId { get; set; }

        public string? StorageRoomName { get; set; }
        public virtual ProductOrderDto? Product { get; set; }
    }
    }
