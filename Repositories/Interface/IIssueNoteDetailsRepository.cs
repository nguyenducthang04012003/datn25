using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IIssueNoteDetailsRepository : IRepository<IssueNoteDetail>
    {
        Task InsertRangeAsync(List<IssueNoteDetail> issueNoteDetailsList);
    }
}
