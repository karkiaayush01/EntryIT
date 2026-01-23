using EntryIt.Common;
using EntryIt.Data;
using EntryIt.Entities;
using Microsoft.EntityFrameworkCore;
using EntryIt.Utils;
namespace EntryIt.Services;

public  class JournalService: IJournalService
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly IStreakService _streakService;

    public JournalService(AppDbContext context, IAuthService authService, IStreakService streakService)
    {
        _context = context;
        _authService = authService;
        _streakService = streakService;
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
            return ServiceResult<SaveResponse>.FailureResult($"An error occurred while saving journal {ex.Message}");
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
}
