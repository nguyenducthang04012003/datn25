using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllAsyncCustomerProduct();
        Task<IEnumerable<Product>> GetAllAsyncProduct();
        Task<Product> GetByIdAsyncProduct(int id);
        Task<IEnumerable<Product>> GetByIdsAsync(List<int> productIds);
    }
}
