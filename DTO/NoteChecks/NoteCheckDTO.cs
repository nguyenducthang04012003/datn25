using PharmaDistiPro.DTO.NoteCheckDetails;

namespace PharmaDistiPro.DTO.NoteChecks
{
    public class NoteCheckDTO
    {
        public int NoteCheckId { get; set; }
        public string? NoteCheckCode { get; set; }
        public int? StorageRoomId { get; set; }
        public string? ReasonCheck { get; set; }
        public string? Result { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public List<NoteCheckDetailsDTO> NoteCheckDetails { get; set; } = new();
    }
}
