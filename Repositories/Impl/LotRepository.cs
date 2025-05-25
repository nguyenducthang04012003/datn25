using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class LotRepository : RepositoryBase<Lot>, ILotRepository 
    {

        public LotRepository(SEP490_G74Context context) : base(context)
        {
        }

        public async Task<Lot> GetLotByLotCode(string LotCode)
        {
            return await _context.Lots.FirstOrDefaultAsync(l => l.LotCode == LotCode);
        }
        public async Task<List<Lot>> GetLotList()
        {
            return await _context.Lots.ToListAsync();
        }

        public async Task<Lot> CreateLot(Lot Lot)
        {
            _context.Lots.Add(Lot);
            int rowAffected = await _context.SaveChangesAsync();
            return Lot;
        }
        public async Task<Lot> UpdateLot(Lot Lot)
        {
            _context.Lots.Update(Lot);
            int rowAffected = await _context.SaveChangesAsync();
            return Lot;
        }
        public async Task<Lot> GetLastLot()
        {
            return await _context.Lots
                .OrderByDescending(l => l.LotCode)
                .FirstOrDefaultAsync();
        }
    }
}
