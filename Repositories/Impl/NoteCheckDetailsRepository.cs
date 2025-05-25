using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class NoteCheckDetailsRepository : RepositoryBase<NoteCheckDetail>, INoteCheckDetailsRepository
    {
        public NoteCheckDetailsRepository(SEP490_G74Context context) : base(context)
        {
        }
    }
}
