using EntryIt.Common;
using EntryIt.Data;
using EntryIt.Entities;
using EntryIt.Models;
using EntryIt.Utils;
using Microsoft.EntityFrameworkCore;

namespace EntryIt.Services;

public class StreakService: IStreakService
{
    private readonly AppDbContext _context;
    private readonly ILoggerService _logger;

    public StreakService (AppDbContext context, ILoggerService logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ServiceResult<StreakUpdateResponse>> IncrementStreak(Guid userId)
    {
        _logger.LogInfo($"IncrementStreak started for UserId: {userId}");

        var userData = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (userData == null)
        {
            _logger.LogInfo($"IncrementStreak failed: user not found (UserId: {userId})");
            return ServiceResult<StreakUpdateResponse>.FailureResult("User data not found");
        }

        DateTime today = DateTimeUtils.GetTodayLocalDate();
        DateTime yesterday = today.AddDays(-1);

        _logger.LogInfo($"Streak check dates — Today: {today:yyyy-MM-dd}, Yesterday: {yesterday:yyyy-MM-dd}");

        Streak? lastStreak = await _context.StreakRecords
            .Where(s => s.UserId == userData.Id)
            .OrderByDescending(s => s.ActivityDate)
            .FirstOrDefaultAsync();

        if (lastStreak != null)
        {
            _logger.LogInfo($"Last streak activity date: {lastStreak.ActivityDate:yyyy-MM-dd}");
        }
        else
        {
            _logger.LogInfo("No previous streak record found");
        }

        // Already counted today
        if (lastStreak?.ActivityDate == today)
        {
            _logger.LogInfo("Streak already counted for today — no changes made");
            return ServiceResult<StreakUpdateResponse>.SuccessResult(
                new StreakUpdateResponse { UpdatedStreak = userData.CurrentStreak }
            );
        }

        // Continue streak
        if (lastStreak?.ActivityDate == yesterday)
        {
            userData.CurrentStreak += 1;
            _logger.LogInfo($"Streak continued. New CurrentStreak: {userData.CurrentStreak}");
        }
        else
        {
            userData.CurrentStreak = 1;
            _logger.LogInfo("Streak reset or started fresh. CurrentStreak set to 1");
        }

        // Update longest streak
        if (userData.CurrentStreak > userData.LongestStreak)
        {
            userData.LongestStreak = userData.CurrentStreak;
            _logger.LogInfo($"New longest streak achieved: {userData.LongestStreak}");
        }

        // Record today's activity
        _context.StreakRecords.Add(new Streak
        {
            UserId = userData.Id,
            ActivityDate = today
        });

        _logger.LogInfo("Streak record added for today");

        await _context.SaveChangesAsync();

        _logger.LogInfo($"IncrementStreak completed successfully. Final CurrentStreak: {userData.CurrentStreak}");

        return ServiceResult<StreakUpdateResponse>.SuccessResult(
            new StreakUpdateResponse { UpdatedStreak = userData.CurrentStreak }
        );
    }


    public async Task<ServiceResult<StreakUpdateResponse>> DecrementStreak(Guid userId)
    {
        try
        {
            var userData = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userData == null) throw new Exception("User not authenticated");

            DateTime today = DateTimeUtils.GetTodayLocalDate();

            // Find today's streak record
            var todayStreak = await _context.StreakRecords
                .Where(s => s.UserId == userData.Id && s.ActivityDate == today)
                .FirstOrDefaultAsync();

            if (todayStreak == null) throw new Exception("No streak record for today to decrement.");

            // Remove today's streak record
            _context.StreakRecords.Remove(todayStreak);

            // Decrement streak, but not below 0
            userData.CurrentStreak = Math.Max(0, userData.CurrentStreak - 1);

            await _context.SaveChangesAsync();

            return ServiceResult<StreakUpdateResponse>.SuccessResult(
                new StreakUpdateResponse { UpdatedStreak = userData.CurrentStreak }
            );
        }
        catch (Exception ex)
        {
            return ServiceResult<StreakUpdateResponse>.FailureResult($"Failed to decrease user streak: {ex.Message}");
        }
    }
}
