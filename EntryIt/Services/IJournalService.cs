using EntryIt.Common;
namespace EntryIt.Services;

public interface IJournalService
{
    public Task<ServiceResult<SaveResponse>> SaveJournal(
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

    public Task<ServiceResult<object?>> DeleteJournal();
}
