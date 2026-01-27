using EntryIt.Common;
using EntryIt.Models;
namespace EntryIt.Services;

public interface IJournalService
{
    /// <summary>
    /// Create/Update tody's journal entry
    /// </summary>
    /// <param name="journalTitle">Title of the journal</param>
    /// <param name="content">Content of the journal</param>
    /// <param name="wordCount">Word Count of the journal</param>
    /// <param name="primaryMood">Primary Mood</param>
    /// <param name="secondaryMood1">Secondary Mood 1</param>
    /// <param name="secondaryMood2">Secondary Mood 2</param>
    /// <param name="lockJournal">Flag to see if journal is locked</param>
    /// <param name="isDefaultPassword">Flag to see if password is default password</param>
    /// <param name="lockPassword">Password for the journal</param>
    /// <param name="tags">Tags for the journal</param>
    /// <returns>Save response with JournalId, Message, and Updated Streak</returns>
    Task<ServiceResult<SaveResponse>> SaveJournal(
        string journalTitle,
        string content, 
        string contentRaw,
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

    /// <summary>
    /// Delete today's journal entry from user database
    /// </summary>
    /// <returns>Success or Failure status</returns>
    Task<ServiceResult<object?>> DeleteJournal();

    Task<ServiceResult<JournalSearchResult>> GetJournalLists(JournalSearchFilters filters);

    Task<ServiceResult<JournalLockStatus>> GetJournalLockStatus(Guid? JournalId);

    Task<ServiceResult<JournalUnlockResponse>> UnlockJournal(Guid JournalId, string password);

    /// <summary>
    /// Export a single journal as PDF
    /// </summary>
    /// <param name="journalId">Journal ID to export</param>
    /// <returns>PDF file as byte array</returns>
    Task<ServiceResult<byte[]>> ExportJournalToPdf(Guid journalId);

    /// <summary>
    /// Export multiple journals by date range as a single PDF
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>PDF file as byte array</returns>
    Task<ServiceResult<byte[]>> ExportJournalsByDateRange(DateTime fromDate, DateTime toDate);

    public event Action? OnChange;
}
