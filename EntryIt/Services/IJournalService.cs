using EntryIt.Common;
namespace EntryIt.Services;

public interface IJournalService
{
    public Task<ServiceResult<SaveResponse>> SaveJournal(
        string journalTitle,
        string content, 
        int wordCount,
        string primaryMood, 
        string secondaryMood1,
        string secondaryMood2,
        bool lockJournal, 
        string lockPassword,
        Guid[] tags
    );

    public Task<ServiceResult<object?>> DeleteJournal();
}
