using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;
using PharmaDistiPro.Repositories.Interface;

namespace PharmaDistiPro.Repositories.Impl
{
    public class IssueNoteDetailsRepository : RepositoryBase<IssueNoteDetail>, IIssueNoteDetailsRepository
    {
        public IssueNoteDetailsRepository(SEP490_G74Context context) : base(context)
        {
        }

        public async Task InsertRangeAsync(List<IssueNoteDetail> issueNoteDetailsList)
        {
            await _context.IssueNoteDetails.AddRangeAsync(issueNoteDetailsList);
        }
    }
}
