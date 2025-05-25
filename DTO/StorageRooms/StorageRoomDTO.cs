using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.DTO.StorageRooms
{
    public class StorageRoomDTO
    {
        public int? StorageRoomId { get; set; }
        public string? StorageRoomCode { get; set; }
        public string? StorageRoomName { get; set; }
        public string? Type { get; set; }
        public double? Capacity { get; set; }
        public double? RemainingRoomVolume { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

       

    }
}
