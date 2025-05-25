using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IProductLotRepository : IRepository<ProductLot>
    {

        Task<IEnumerable<ProductLot>> GetProductLotsByProductIds(List<int> productIds);
        Task<List<ProductLot>> GetProductLotList();
        Task<ProductLot> GetProductLotById(int id);
        Task<List<ProductLot>> CreateProductLots(List<ProductLot> ProductLots);
        Task<ProductLot> CreateProductLot(ProductLot productLot);
        Task<ProductLot> UpdateProductLot(ProductLot ProductLot);
        Task<Lot> GetLotById(int id);
        Task<Product> GetProductByIdAsync(int productId);
        Task UpdateAsyncProductLot(ProductLot productLot);

        Task<int> CheckQuantityProduct(int productId);

    }
}
