using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class UnitRepository : RepositoryBase<Unit>, IUnitRepository
    {
        public UnitRepository(SEP490_G74Context context) : base(context)
        {
        }
    }
}
