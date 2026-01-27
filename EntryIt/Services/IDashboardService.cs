using EntryIt.Common;
using EntryIt.Models;

namespace EntryIt.Services;

public interface IDashboardService
{
    Task<ServiceResult<List<MoodDistribution>>> GetMoodDistribution(FilterDates filters);

    Task<ServiceResult<List<WordCountDistributions>>> GetWordCountDistributions(FilterDates filters);

    Task<ServiceResult<List<StreakRecord>>> GetCurrentMonthStreak(int month);
}