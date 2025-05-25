using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(SEP490_G74Context context) : base(context)
        {
        }

        // Lấy tất cả sản phẩm với thông tin hình ảnh và category
        public async Task<IEnumerable<Product>> GetAllAsyncProduct()
        {
            return await _context.Products
                .Include(p => p.Category) // Bao gồm thông tin Category
                .Include(p => p.ImageProducts) // Bao gồm thông tin hình ảnh
                .ToListAsync();
        }

        
        public async Task<Product> GetByIdAsyncProduct(int id)
        {
         

            return await _context.Products
                .Include(p => p.Category) 
                .Include(p => p.ImageProducts) 
                .FirstOrDefaultAsync(p => p.ProductId == id); 
        }


        public async Task<IEnumerable<Product>> GetAllAsyncCustomerProduct()
        {
            return await _context.Products.Where(s=>s.Status == true)
                .Include(p => p.Category) // Bao gồm thông tin Category
                .Include(p => p.ImageProducts) // Bao gồm thông tin hình ảnh
                .ToListAsync();
        }


        public async Task<IEnumerable<Product>> GetByIdsAsync(List<int> productIds)
        {
            return await _context.Products
                                 .Where(p => productIds.Contains(p.ProductId))
                                 .ToListAsync();
        }
    }

}
