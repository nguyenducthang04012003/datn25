

namespace PharmaDistiPro.DTO.NoteChecks
{
    public class NoteCheckRequestDTO
    {
        public int NoteCheckId { get; set; }
        public string? NoteCheckCode { get; set; }

        public int? StorageRoomId { get; set; }
        public string? ReasonCheck { get; set; }
        public string? Result { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }


        public List<NoteCheckDetailRequestDTO> NoteCheckDetails { get; set; } = new();
    }

    public class NoteCheckDetailRequestDTO
    {
        public int NoteCheckDetailId { get; set; }
        public int? NoteCheckId { get; set; }
        public int? ProductLotId { get; set; }
        public int? StorageQuantity { get; set; }

        public int? ActualQuantity { get; set; }
        public int? DifferenceQuatity { get; set; }
        public int? ErrorQuantity { get; set; }
        public int? Status { get; set; }
    }


}
