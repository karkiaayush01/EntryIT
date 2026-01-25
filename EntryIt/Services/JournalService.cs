using EntryIt.Common;
using EntryIt.Data;
using EntryIt.Entities;
using Microsoft.EntityFrameworkCore;
using EntryIt.Utils;
using EntryIt.Models;
namespace EntryIt.Services;

public  class JournalService: IJournalService
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly IStreakService _streakService;
    private readonly ILoggerService _loggerService;

    // OnChange Event. Used to notify users to refetch their journal content
    public event Action? OnChange;

    public JournalService(AppDbContext context, IAuthService authService, IStreakService streakService, ILoggerService loggerService)
    {
        _context = context;
        _authService = authService;
        _streakService = streakService;
        _loggerService = loggerService;
    }

    private JournalViewModel MapJournalToViewModel(Journal journal, List<Tag> journalTags)
    {
        return new JournalViewModel
        {
            Id = journal.Id,
            Title = journal.Title,
            Content = journal.Content,
            WordCount = journal.WordCount,
            PrimaryMood = journal.PrimaryMood,
            SecondaryMood1 = journal.SecondaryMood1,
            SecondaryMood2  = journal.SecondaryMood2,
            IsLocked = journal.IsLocked,
            SaveDate = journal.SaveDate,
            LastUpdatedAt = journal.LastUpdatedAt,
            SelectedTags = journalTags
        };
    }

    public async Task<ServiceResult<SaveResponse>> SaveJournal(
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
    )
    {
        try
        {
            //Simulate API
            await Task.Delay(2000);
            _loggerService.LogInfo($"Received tags: {string.Join(",", tags)}");
            var user = _authService.GetCurrentUser();

            if (isDefaultPassword && user != null)
            {
                lockPassword = (await _context.Users.FirstAsync(u => u.Id == user.Id)).JournalLockPassword;
            }

            SaveResponse result = new SaveResponse();

            if (user == null)
            {
                return ServiceResult<SaveResponse>.FailureResult($"Cannot save journal. User not authenticated");
            }

            // Get today's journal to see if its a new or update
            DateTime today = DateTime.Now.Date;

            var existingJournal = await _context.Journals.FirstOrDefaultAsync(
                j => j.CreatedBy == user.Id && j.SaveDate == today
            );

            if(lockJournal)
            {
                lockPassword = BCrypt.Net.BCrypt.HashPassword(lockPassword);
            } else
            {
                lockPassword = string.Empty;
            }

            if (existingJournal == null)
            {
                Journal newJournal = new Journal
                {
                    Title = journalTitle,
                    Content = content,
                    CreatedBy = user.Id,
                    WordCount = wordCount,
                    PrimaryMood = primaryMood,
                    SecondaryMood1 = secondaryMood1,
                    SecondaryMood2 = secondaryMood2,
                    IsLocked = lockJournal,
                    Password = lockPassword,
                    SaveDate = today
                };

                // Update user activity and streak from Streak Service
                int updatedStreak = user.CurrentStreak;
                var streakUpdateResult = await _streakService.IncrementStreak(user.Id);
                if (streakUpdateResult.Success && streakUpdateResult.Data != null)
                {
                    updatedStreak = streakUpdateResult.Data.UpdatedStreak;
                }

                _context.Journals.Add(newJournal);
                await _context.SaveChangesAsync();


                foreach (Guid tagId in tags)
                {
                    var journalTag = new Journal_Tag
                    {
                        JournalId = newJournal.Id,
                        TagId = tagId
                    };
                    _context.Journal_Tags.Add(journalTag);
                }
                await _context.SaveChangesAsync();

                result = new SaveResponse
                {
                    JournalId = newJournal.Id,
                    Message = "Journal Entry Created Successfully",
                    UpdatedStreak = updatedStreak
                };
            }
            else
            {
                existingJournal.Title = journalTitle;
                existingJournal.Content = content;
                existingJournal.WordCount = wordCount;
                existingJournal.PrimaryMood = primaryMood;
                existingJournal.SecondaryMood1 = secondaryMood1;
                existingJournal.SecondaryMood2 = secondaryMood2;
                existingJournal.IsLocked = lockJournal;
                existingJournal.Password = lockPassword;
                existingJournal.LastUpdatedAt = DateTime.UtcNow;

                // Update the tags by removing the tags that user removed and new tags user added
                var currentTagIds = await _context.Journal_Tags
                .Where(jt => jt.JournalId == existingJournal.Id)
                .Select(jt => jt.TagId)
                .ToListAsync();


                var tagsToRemove = currentTagIds.Except(tags).ToList();
                if (tagsToRemove.Any())
                {
                    var jtToRemove = await _context.Journal_Tags
                    .Where(jt => jt.JournalId == existingJournal.Id && tagsToRemove.Contains(jt.TagId))
                    .ToListAsync();


                    _context.Journal_Tags.RemoveRange(jtToRemove);
                }

                var tagsToAdd = tags.Except(currentTagIds).ToList();
                foreach (var tagId in tagsToAdd)
                {
                    _context.Journal_Tags.Add(new Journal_Tag
                    {
                        JournalId = existingJournal.Id,
                        TagId = tagId
                    });
                }


                // Save changes
                await _context.SaveChangesAsync();

                result = new SaveResponse
                {
                    JournalId = existingJournal.Id,
                    Message = "Journal Entry Updated Successfully",
                    UpdatedStreak = user.CurrentStreak
                };
            }
            
            return ServiceResult<SaveResponse>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<SaveResponse>.FailureResult($"An error occurred while saving journal: {ex.Message}");
        }
    }

    // Only need success or failure status so no need for a response model
    public async Task<ServiceResult<object?>> DeleteJournal()
    {
        try
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not authenticated");
            }

            // Get today's journal
            DateTime today = DateTimeUtils.GetUtcDateTime();

            Journal? journalToday = await _context.Journals
                .Where(j => j.CreatedBy == currentUser.Id && j.SaveDate == today)
                .FirstOrDefaultAsync();
            
            if (journalToday == null)
            {
                throw new Exception("No journal found for today.");
            } 
            else
            {
                List<Journal_Tag> tagsToDelete = _context.Journal_Tags.Where(t => t.JournalId == journalToday.Id).ToList();
                _context.Journal_Tags.RemoveRange(tagsToDelete);
                _context.Journals.Remove(journalToday);

                // Update user activity and streak from Streak Service
                int updatedStreak = currentUser.CurrentStreak;
                var streakUpdateResult = await _streakService.DecrementStreak(currentUser.Id);
                if (streakUpdateResult.Success && streakUpdateResult.Data != null)
                {
                    updatedStreak = streakUpdateResult.Data.UpdatedStreak;
                }

                await _context.SaveChangesAsync();
            }
            return ServiceResult<object?>.SuccessResult(null);
        }
        catch (Exception ex)
        {
            return ServiceResult<object?>.FailureResult($"Failed to delete Journal: {ex.Message}");
        }
    }

    private async Task<List<Tag>> GetJournalTags(Guid JournalId)
    {
        try
        {
            List<Tag> tags = await (
                from t in _context.Tags
                join jt in _context.Journal_Tags on t.Id equals jt.TagId
                where jt.JournalId == JournalId
                select t
            ).ToListAsync();

            return tags;
        } 
        catch (Exception ex)
        {
            _loggerService.LogError($"Failed to get journal Tags: {ex.Message}");
            return [];
        }
    }

    public async Task<ServiceResult<JournalViewModel>> GetJournal(bool today, Guid userId, Guid journalId = default)
    {
        try
        {
            // Throw if no user id
            if(userId == Guid.Empty) { throw new ArgumentNullException("Missing userId. Please authenticate first."); }

            // Check if its the authenticated user
            if(userId != _authService.GetCurrentUser()?.Id) { throw new Exception("You are not authorized to get this journal"); }

            if(!today && journalId == Guid.Empty)
            {
                throw new ArgumentNullException("Journal Id is needed for journals not today");
            }

            JournalViewModel mappedResult = new JournalViewModel{};
            Journal? journal = new Journal();
            
            if(today)
            {
                // Get today's journal
                DateTime todayDate = DateTimeUtils.GetTodayLocalDate();
                journal = await _context.Journals
                    .Where(j => j.SaveDate == todayDate && j.CreatedBy == userId)
                    .FirstOrDefaultAsync();

                if (journal == null)
                {
                    throw new Exception("Could not find any journal for today");
                }
            }
            else
            {
                // Get specified journal from journal Id
                journal = await _context.Journals
                    .Where(j => j.Id == journalId && j.CreatedBy == userId)
                    .FirstOrDefaultAsync();

                if (journal == null)
                {
                    throw new Exception("Could not find the specific journal. Perhaps it belongs to different user.");
                }
            }

            List<Tag> journalTags = await GetJournalTags(journal.Id);
            mappedResult = MapJournalToViewModel(journal, journalTags);
            return ServiceResult<JournalViewModel>.SuccessResult(mappedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<JournalViewModel>.FailureResult($"Failed to retrieve journal {ex.Message}");
        }
    }

    /// <summary>
    /// Delete today's journal
    /// </summary>
    /// <param name="userId">User who requested to delete their journal</param>
    /// <param name="journalId">Journal Id to be deleted</param>
    /// <returns>Success or Failure based on delete operation</returns>
    /// <exception cref="ArgumentNullException">Missing userId or JournalId</exception>
    public async Task<ServiceResult<object?>> DeleteJournal(Guid userId, Guid journalId)
    {
        try
        {
            // Argument validations
            if(userId == Guid.Empty || journalId == Guid.Empty)
            {
                throw new ArgumentNullException($"Missing user id or journal id");
            }

            DateTime today = DateTimeUtils.GetTodayLocalDate();

            Journal? journal = await _context.Journals
                .Where(j => j.CreatedBy == userId && j.Id == journalId)
                .FirstOrDefaultAsync();

            if (journal == null)
            {
                throw new Exception($"Could not find journal");
            }
            else if (journal.SaveDate != today)
            {
                throw new Exception("Cannot delete journal that was not created today");
            }
            else
            {
                // Delete the journal
                _context.Journals.Remove(journal);
                await _context.SaveChangesAsync();

                return ServiceResult<object?>.SuccessResult(null);
            }
        }
        catch (Exception ex)
        {
            return ServiceResult<object?>.FailureResult($"An error occurred while deleting journal: {ex.Message}");
        }
    }

    /// <summary>
    /// Notify Users of the state change to refetch their data.
    /// </summary>
    private void NotifyStateChanged() => OnChange?.Invoke();
}

