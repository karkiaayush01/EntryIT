using EntryIt.Common;
using EntryIt.Data;
using EntryIt.Entities;
using EntryIt.Models;
using Microsoft.EntityFrameworkCore;

namespace EntryIt.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILoggerService _loggerService;

    public DashboardService(AppDbContext context, IAuthService authService, ILoggerService loggerService)
    {
        _context = context;
        _authService = authService;
        _loggerService = loggerService;
    }

    /// <summary>
    /// Gets the distribution of moods for the authenticated user's journals within the specified date range.
    /// </summary>
    /// <param name="filters">Filter criteria containing FromDate and ToDate for the date range.</param>
    /// <returns>A <see cref="ServiceResult{List{MoodDistribution}}"/> containing a list of mood distributions with counts.</returns>
    public async Task<ServiceResult<List<MoodDistribution>>> GetMoodDistribution(FilterDates filters)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return ServiceResult<List<MoodDistribution>>.FailureResult("User not authenticated");
            }

            // Build query for journals within date range
            IQueryable<Journal> journalQuery = _context.Journals
                .Where(j => j.CreatedBy == currentUser.Id);

            // Apply date filters
            if (filters.FromDate.HasValue)
            {
                journalQuery = journalQuery.Where(j => j.SaveDate >= filters.FromDate.Value.Date);
            }

            journalQuery = journalQuery.Where(j => j.SaveDate <= filters.ToDate.Date);

            // Get all journals in date range
            var journals = await journalQuery.ToListAsync();

            // Collect all mood IDs (primary and secondary)
            var allMoodIds = new List<Guid>();

            foreach (var journal in journals)
            {
                allMoodIds.Add(journal.PrimaryMood);

                if (journal.SecondaryMood1 != Guid.Empty)
                {
                    allMoodIds.Add(journal.SecondaryMood1);
                }

                if (journal.SecondaryMood2 != Guid.Empty)
                {
                    allMoodIds.Add(journal.SecondaryMood2);
                }
            }

            // Group by mood ID and count
            var moodCounts = allMoodIds
                .GroupBy(id => id)
                .Select(g => new { MoodId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Get unique mood IDs
            var uniqueMoodIds = moodCounts.Select(mc => mc.MoodId).Distinct().ToList();

            // Batch fetch all moods
            var moods = await _context.Moods
                .Where(m => uniqueMoodIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            // Build result
            List<MoodDistribution> result = new List<MoodDistribution>();

            foreach (var moodCount in moodCounts)
            {
                var mood = moods.GetValueOrDefault(moodCount.MoodId);
                if (mood != null)
                {
                    result.Add(new MoodDistribution
                    {
                        Mood = new MoodViewModel
                        {
                            Id = mood.Id,
                            Name = mood.Name,
                            Category = mood.Category,
                            Emoji = mood.Emoji
                        },
                        MoodCount = moodCount.Count
                    });
                }
            }

            return ServiceResult<List<MoodDistribution>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Failed to get mood distribution: {ex.Message}");
            return ServiceResult<List<MoodDistribution>>.FailureResult($"Failed to get mood distribution: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the word count distribution for the authenticated user's journals within the specified date range.
    /// </summary>
    /// <param name="filters">Filter criteria containing FromDate and ToDate for the date range.</param>
    /// <returns>A <see cref="ServiceResult{List{WordCountDistributions}}"/> containing word counts per date.</returns>
    public async Task<ServiceResult<List<WordCountDistributions>>> GetWordCountDistributions(FilterDates filters)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return ServiceResult<List<WordCountDistributions>>.FailureResult("User not authenticated");
            }

            // Build query for journals within date range
            IQueryable<Journal> journalQuery = _context.Journals
                .Where(j => j.CreatedBy == currentUser.Id);

            // Apply date filters
            if (filters.FromDate.HasValue)
            {
                journalQuery = journalQuery.Where(j => j.SaveDate >= filters.FromDate.Value.Date);
            }

            journalQuery = journalQuery.Where(j => j.SaveDate <= filters.ToDate.Date);

            // Get word counts grouped by date
            var wordCounts = await journalQuery
                .OrderBy(j => j.SaveDate)
                .Select(j => new WordCountDistributions
                {
                    Date = j.SaveDate,
                    WordCount = j.WordCount
                })
                .ToListAsync();

            return ServiceResult<List<WordCountDistributions>>.SuccessResult(wordCounts);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Failed to get word count distributions: {ex.Message}");
            return ServiceResult<List<WordCountDistributions>>.FailureResult($"Failed to get word count distributions: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the streak records (activity dates) for the authenticated user for a specific month.
    /// </summary>
    /// <param name="month">The month number (1-12) to retrieve streak records for.</param>
    /// <returns>A <see cref="ServiceResult{List{StreakRecord}}"/> containing dates when the user had journal activity.</returns>
    public async Task<ServiceResult<List<StreakRecord>>> GetCurrentMonthStreak(int month)
    {
        try
        {
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return ServiceResult<List<StreakRecord>>.FailureResult("User not authenticated");
            }

            // Validate month
            if (month < 1 || month > 12)
            {
                return ServiceResult<List<StreakRecord>>.FailureResult("Invalid month. Month must be between 1 and 12.");
            }

            // Get current year
            int currentYear = DateTime.Now.Year;

            // Get start and end dates for the month
            DateTime startDate = new DateTime(currentYear, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // Get all journal dates for the user in this month
            var streakRecords = await _context.Journals
                .Where(j => j.CreatedBy == currentUser.Id
                    && j.SaveDate >= startDate
                    && j.SaveDate <= endDate)
                .Select(j => new StreakRecord
                {
                    ActivityDate = j.SaveDate
                })
                .OrderBy(sr => sr.ActivityDate)
                .ToListAsync();

            return ServiceResult<List<StreakRecord>>.SuccessResult(streakRecords);
        }
        catch (Exception ex)
        {
            _loggerService.LogError($"Failed to get current month streak: {ex.Message}");
            return ServiceResult<List<StreakRecord>>.FailureResult($"Failed to get current month streak: {ex.Message}");
        }
    }
}