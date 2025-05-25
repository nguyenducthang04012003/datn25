using PharmaDistiPro.DTO.ProductLots;
using PharmaDistiPro.DTO.StorageRooms;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Services.Interface
{
    public interface IProductLotService
    {
        Task<Services.Response<List<ProductLotResponse>>> GetProductLotList();
        Task<Response<List<ProductLotResponse>>> CreateProductLot(List<ProductLotRequest> productLots);

        Task<Services.Response<ProductLotResponse>> UpdateProductLot(ProductLotRequest ProductLot);

        Task<Services.Response<ProductLotResponse>> GetProductLotById(int id);

        Task<Services.Response<int>> CheckQuantityProduct(int productId);

        Task<Response<int>> GetQuantityByProductIdAsync(int productId);
        Task<Response<IEnumerable<StorageRoomDTO>>> ListCompatibleStorageRoomsAsync(int productLotId);
        Task<Response<List<ProductLotResponse>>> GetProductLotsByStatusAsync(int status);
        Task SendProductLotStatusChangeNotificationAsync(List<ProductLotResponse> changedLots);
        Task<Response<List<ProductLotResponse>>> AutoUpdateProductLotStatusAsync();
    }
}
