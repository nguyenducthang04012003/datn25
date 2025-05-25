using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class NoteCheckRepository : RepositoryBase<NoteCheck>, INoteCheckRepository
    {

        public NoteCheckRepository(SEP490_G74Context context) : base(context)
        {

        }

        public async Task UpdateDetailAsync(NoteCheckDetail noteCheckDetail)
        {
            _context.NoteCheckDetails.Update(noteCheckDetail);
            await _context.SaveChangesAsync();
        }
        public async Task<NoteCheckDetail> GetDetailByIdAsync(int noteCheckDetailId)
        {
            return await _context.NoteCheckDetails
                .Include(ncd => ncd.ProductLot)
                .FirstOrDefaultAsync(ncd => ncd.NoteCheckDetailId == noteCheckDetailId);
        }
        public async Task InsertNoteCheckAsync(NoteCheck notecheck)
        {
            
            var createdDate = notecheck.CreatedDate ?? DateTime.Now;

            var maxNoteCheckNumber = await GetMaxNoteCheckNumberByDate(createdDate);

            
            var nextNoteCheckNumber = (maxNoteCheckNumber + 1).ToString();

            
            if (maxNoteCheckNumber + 1 < 100)
            {
                nextNoteCheckNumber = nextNoteCheckNumber.PadLeft(3, '0');
            }

          
            notecheck.NoteCheckCode = $"{ConstantStringHelper.NoteCheckCode}{createdDate:ddMMyyyy}{nextNoteCheckNumber}";

           
            await _context.NoteChecks.AddAsync(notecheck);
            await _context.SaveChangesAsync();
        }

       
        public async Task<int> GetMaxNoteCheckNumberByDate(DateTime createdDate)
        {
          
            var noteCheckCodePattern = $"{ConstantStringHelper.NoteCheckCode}{createdDate:ddMMyyyy}";

          
            var latestNoteCheck = await _context.NoteChecks
                .Where(o => o.CreatedDate.HasValue &&
                            o.CreatedDate.Value.Year == createdDate.Year &&
                            o.CreatedDate.Value.Month == createdDate.Month &&
                            o.CreatedDate.Value.Day == createdDate.Day &&
                            o.NoteCheckCode.StartsWith(noteCheckCodePattern)) 
                .OrderByDescending(o => o.NoteCheckCode)
                .FirstOrDefaultAsync();

            
            if (latestNoteCheck == null)
                return 0;

            
            var lastNoteCheckNumberStr = latestNoteCheck.NoteCheckCode.Substring(noteCheckCodePattern.Length);

            return int.TryParse(lastNoteCheckNumberStr, out int noteCheckNumber) ? noteCheckNumber : 0;
        }


        public async Task<List<NoteCheckDetail>> GetDetailsByNoteCheckIdAsync(int noteCheckId)
        {
            
            return await _context.NoteCheckDetails
                                   .Where(d => d.NoteCheckId == noteCheckId)
                                   .ToListAsync();
        }
        public async Task<NoteCheck> GetNoteCheckByIdAsync(int noteCheckId)
        {
            return await _context.NoteChecks
                .Include(nc => nc.NoteCheckDetails) // Nạp quan hệ NoteCheckDetails
                .FirstOrDefaultAsync(nc => nc.NoteCheckId == noteCheckId);
        }

        public async Task<List<NoteCheck>> GetAllWithDetailsAsync()
        {
            return await _context.NoteChecks
                .Include(nc => nc.NoteCheckDetails)
                    .ThenInclude(ncd => ncd.ProductLot)
                        .ThenInclude(pl => pl.Product)
                .ToListAsync();
        }

        public async Task UpdateNoteCheckAsync(NoteCheck noteCheck)
        {
            _context.NoteChecks.Update(noteCheck);
            await _context.SaveChangesAsync();
        }

        public async Task<NoteCheck> GetByCodeWithDetailsAsync(string noteCheckCode)
        {
            return await _context.NoteChecks
                .Include(n => n.NoteCheckDetails)
                .ThenInclude(d => d.ProductLot)
                .ThenInclude(pl => pl.Product)
                .FirstOrDefaultAsync(n => n.NoteCheckCode == noteCheckCode);
        }

    }
}
