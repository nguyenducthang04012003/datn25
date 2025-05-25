using PharmaDistiPro.DTO.StorageRooms;

namespace PharmaDistiPro.Services.Interface
{
    public interface IStorageHistoryService
    {
        Task<StorageHistoryDTO> CreateStorageHistoryAsync(StorageHistoryInputRequest request);
        Task<StorageHistoryChartDTO> GetLatestByStorageRoomIdAsync(int storageRoomId);
        Task<List<StorageHistoryChartDTO>> GetTop50EarliestForChartAsync(int storageRoomId);
        Task<bool> HasSensorAsync(int storageRoomId);
    }
}
