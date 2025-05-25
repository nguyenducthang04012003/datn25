using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface ILotRepository : IRepository<Lot>
    {
        Task<Lot> GetLotByLotCode(string LotCode);
        Task<List<Lot>> GetLotList();
        Task<Lot> GetLastLot();
        Task<Lot> CreateLot(Lot Lot);
        Task<Lot> UpdateLot(Lot Lot);


    }
}
