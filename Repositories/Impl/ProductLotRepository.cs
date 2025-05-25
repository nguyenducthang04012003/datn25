using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;
using System.Linq;

namespace PharmaDistiPro.Repositories.Impl
{
    public class ProductLotRepository : RepositoryBase<ProductLot>, IProductLotRepository
    {
        public ProductLotRepository(SEP490_G74Context context) : base(context)
        {
        }
        public async Task<IEnumerable<ProductLot>> GetProductLotsByProductIds(List<int> productIds)
        {
            return await _context.ProductLots
                .Where(p => p.ProductId.HasValue && productIds.Contains(p.ProductId.Value) &&( p.Status == 1 || p.Status==2))
                .OrderBy(p => p.ExpiredDate)
                .ToListAsync();
        }
        public async Task<List<ProductLot>> CreateProductLots(List<ProductLot> productLots)
        {
            await _context.ProductLots.AddRangeAsync(productLots); // Corrected AddRangeAsync
            await _context.SaveChangesAsync(); // Save changes to database

            return productLots; // Return the inserted product lots
        }
        public async Task<ProductLot> CreateProductLot(ProductLot productLot)
        {
            await _context.ProductLots.AddAsync(productLot); // Corrected AddRangeAsync
            await _context.SaveChangesAsync(); // Save changes to database

            return productLot; // Return the inserted product lots
        }
        public async Task<Lot> GetLotById(int id)
        {
            return await _context.Lots.FirstOrDefaultAsync(x => x.LotId == id);
        }

        public async Task<ProductLot> GetProductLotById(int id)
        {
            return await _context.ProductLots.Include(pl => pl.Product).Include(pl => pl.Lot).FirstOrDefaultAsync(x => x.ProductLotId == id);
        }

        public async Task<List<ProductLot>> GetProductLotList()
        {
            return await _context.ProductLots.Include(pl => pl.Product).Include(pl => pl.Lot).ToListAsync();
        }

        public async Task<ProductLot> UpdateProductLot(ProductLot ProductLot)
        {
            _context.ProductLots.Update(ProductLot);
            int rowAffected = await _context.SaveChangesAsync();
            return await GetProductLotById(ProductLot.ProductLotId);
        }
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Category) // Bao gồm thông tin Category
                .Include(p => p.ImageProducts)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }


        public async Task UpdateAsyncProductLot(ProductLot productLot)
        {
            _context.ProductLots.Update(productLot);
            await _context.SaveChangesAsync(); // Phải có dòng này
        }

        public async Task<int> CheckQuantityProduct(int productId)
        {
            var productLots = await _context.ProductLots.ToListAsync();

            var totalOrderedQuantity = await _context.OrdersDetails
        .Where(x => x.ProductId == productId &&
                ( x.Order.Status == (int)Common.Enums.OrderStatus.XAC_NHAN ||
                  x.Order.Status == (int)Common.Enums.OrderStatus.DANG_CHO_XAC_NHAN ||
                   x.Order.Status == (int)Common.Enums.OrderStatus.DANG_CHO_THANH_TOAN)
                 )
        .SumAsync(x => x.Quantity ?? 0);

            var totalQuantity = productLots
                .Where(lot => lot.ProductId == productId && (lot.Status == 1 || lot.Status == 2))
                .Sum(lot => lot.Quantity ?? 0);

            var QuantityReserved = totalQuantity - totalOrderedQuantity;

            return QuantityReserved;
        }
    }
}
