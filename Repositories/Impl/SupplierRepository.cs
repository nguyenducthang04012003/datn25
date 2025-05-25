using Microsoft.EntityFrameworkCore;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class SupplierRepository : RepositoryBase<Supplier>  ,ISupplierRepository
    {
        public SupplierRepository(SEP490_G74Context context) : base(context)
        {
        }
       
    }
}
