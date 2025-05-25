namespace PharmaDistiPro.DTO.StorageRooms
{
    public class StorageHistoryInputRequest
    {
        public int StorageRoomId { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Service { get; set; }
    }
}
