using EntryIt.Common;
using EntryIt.Models;
namespace EntryIt.Services;

public interface IJournalService
{
    Task<ServiceResult<SaveResponse>> SaveJournal(
        string journalTitle,
        string content, 
        int wordCount,
        Guid primaryMood, 
        Guid secondaryMood1,
        Guid secondaryMood2,
        bool lockJournal, 
        bool isDefaultPassword,
        string lockPassword,
        List<Guid> tags
    );

    Task<ServiceResult<JournalViewModel>> GetJournal(bool today, Guid userId, Guid journalId);

    Task<ServiceResult<object?>> DeleteJournal(Guid userId, Guid journalId);
}
