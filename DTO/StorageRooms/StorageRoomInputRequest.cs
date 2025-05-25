namespace PharmaDistiPro.DTO.StorageRooms
{
    public class StorageRoomInputRequest
    {
        public int? StorageRoomId { get; set; }
        public string? StorageRoomCode { get; set; }
        public string? StorageRoomName { get; set; }
        public int? Type { get; set; }
        public double? Capacity { get; set; }
        public double? RemainingRoomVolume { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
