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
            ConentRaw = journal.ContentRaw,
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
        string contentRaw,
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

            SaveResponse result = new();

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
                Journal newJournal = new()
                {
                    Title = journalTitle,
                    Content = content,
                    ContentRaw = contentRaw,
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
                existingJournal.ContentRaw = contentRaw;
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
            //Simulate API
            await Task.Delay(2000);
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new Exception("User not authenticated");
            }

            // Get today's journal
            DateTime today = DateTimeUtils.GetTodayLocalDate();

            Journal? journalToday = await _context.Journals
                .Where(j => j.CreatedBy == currentUser.Id && j.SaveDate == today)
                .FirstOrDefaultAsync();
            
            if (journalToday == null)
            {
                throw new Exception("No journal found for today.");
            } 
            else
            {
                // Remove all the Tag entries
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

            JournalViewModel mappedResult = new JournalViewModel { };
            Journal? journal = new();
            
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

    public async Task<ServiceResult<JournalSearchResult>> GetJournalLists(JournalSearchFilters filters)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return ServiceResult<JournalSearchResult>.FailureResult("User not authenticated");
            }

            // Build the base query
            IQueryable<Journal> query = _context.Journals
                .Where(j => j.CreatedBy == currentUser.Id);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(filters.SearchKey))
            {
                string searchTerm = filters.SearchKey.ToLower();
                query = query.Where(j =>
                    j.Title.ToLower().Contains(searchTerm) ||
                    (j.ContentRaw.ToLower().Contains(searchTerm) && j.IsLocked == false)
                );
            }

            // Apply date range filters
            if (filters.FromDate.HasValue)
            {
                query = query.Where(j => j.SaveDate >= filters.FromDate.Value.Date);
            }

            query = query.Where(j => j.SaveDate <= filters.ToDate.Date);

            // Order by SaveDate descending (most recent first)
            query = query.OrderByDescending(j => j.SaveDate);

            // Get total count for pagination
            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)filters.PerPage);

            // Apply pagination
            int skip = (filters.Page - 1) * filters.PerPage;
            var journals = await query
                .Skip(skip)
                .Take(filters.PerPage)
                .ToListAsync();

            if (!journals.Any())
            {
                return ServiceResult<JournalSearchResult>.SuccessResult(new JournalSearchResult
                {
                    Results = new List<JournalSearchResponse>(),
                    CurrentPage = filters.Page,
                    TotalPages = 0
                });
            }

            // Get all journal IDs for batch operations
            var journalIds = journals.Select(j => j.Id).ToList();

            // Batch fetch all tags for all journals
            var allJournalTags = await (
                from jt in _context.Journal_Tags
                join t in _context.Tags on jt.TagId equals t.Id
                where journalIds.Contains(jt.JournalId)
                select new { jt.JournalId, Tag = t }
            ).ToListAsync();

            var journalTagsDict = allJournalTags
                .GroupBy(x => x.JournalId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Tag).ToList());

            // Get all unique mood IDs
            var moodIds = journals
                .SelectMany(j => new[] { j.PrimaryMood, j.SecondaryMood1, j.SecondaryMood2 })
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            // Batch fetch all moods
            var moods = await _context.Moods
                .Where(m => moodIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            // Map to response model
            List<JournalSearchResponse> results = new List<JournalSearchResponse>();

            foreach (var journal in journals)
            {
                JournalSearchResponse response = new JournalSearchResponse
                {
                    JournalId = journal.Id,
                    Title = journal.Title,
                    SaveDate = journal.SaveDate,
                    IsLocked = journal.IsLocked
                };

                // Only include journal info if not locked
                if (!journal.IsLocked)
                {
                    // Get tags for this journal
                    var tags = journalTagsDict.GetValueOrDefault(journal.Id, new List<Tag>());

                    // Get mood data from dictionary
                    var primaryMood = moods.GetValueOrDefault(journal.PrimaryMood);
                    var secondaryMood1 = journal.SecondaryMood1 != Guid.Empty
                        ? moods.GetValueOrDefault(journal.SecondaryMood1)
                        : null;
                    var secondaryMood2 = journal.SecondaryMood2 != Guid.Empty
                        ? moods.GetValueOrDefault(journal.SecondaryMood2)
                        : null;

                    response.JournalInfo = new JournalExtraInfo
                    {
                        Content = journal.Content,
                        ContentRaw = journal.ContentRaw,
                        WordCount = journal.WordCount,
                        PrimaryMood = primaryMood != null ? new MoodViewModel
                        {
                            Id = primaryMood.Id,
                            Name = primaryMood.Name,
                            Category = primaryMood.Category,
                            Emoji = primaryMood.Emoji
                        } : new MoodViewModel(),
                        SecondaryMood1 = secondaryMood1 != null ? new MoodViewModel
                        {
                            Id = secondaryMood1.Id,
                            Name = secondaryMood1.Name,
                            Category = secondaryMood1.Category,
                            Emoji = secondaryMood1.Emoji
                        } : null,
                        SecondaryMood2 = secondaryMood2 != null ? new MoodViewModel
                        {
                            Id = secondaryMood2.Id,
                            Name = secondaryMood2.Name,
                            Category = secondaryMood2.Category,
                            Emoji = secondaryMood2.Emoji
                        } : null,
                        SelectedTags = tags
                    };
                }

                results.Add(response);
            }

            var searchResult = new JournalSearchResult
            {
                Results = results,
                CurrentPage = filters.Page,
                TotalPages = totalPages
            };

            return ServiceResult<JournalSearchResult>.SuccessResult(searchResult);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Failed to get journal lists: {ex.Message}");
            return ServiceResult<JournalSearchResult>.FailureResult($"Failed to get results: {ex.Message}");
        }
    }

    /// <summary>
    /// Notify Users of the state change to refetch their data.
    /// </summary>
    private void NotifyStateChanged() => OnChange?.Invoke();
}

