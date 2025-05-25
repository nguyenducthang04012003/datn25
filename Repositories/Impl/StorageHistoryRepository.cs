using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class StorageHistoryRepository : RepositoryBase<StorageHistory>, IStorageHistoryRepository
    {
        public StorageHistoryRepository(SEP490_G74Context context) : base(context)
        {
        }
    }

}
