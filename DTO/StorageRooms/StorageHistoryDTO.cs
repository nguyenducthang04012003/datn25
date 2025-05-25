using PharmaDistiPro.Models;
using System.Text.Json.Serialization;
namespace PharmaDistiPro.DTO.StorageRooms
{
    public class StorageHistoryDTO
    {
        public int Id { get; set; }
        public int? StorageRoomId { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Service { get; set; }
        public string? AlertMessage { get; set; }
        public string? AlertDetail { get; set; }

        [JsonIgnore]
        public virtual StorageRoom? StorageRoom { get; set; }
    }
}
