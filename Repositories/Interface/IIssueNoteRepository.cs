using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Infrastructures;

namespace PharmaDistiPro.Repositories.Interface
{
    public interface IIssueNoteRepository : IRepository<IssueNote>
    {
        Task CreateIssueNote(IssueNote issueNote);
    }
}
