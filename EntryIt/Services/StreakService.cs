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

    public StreakService (AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<StreakUpdateResponse>> IncrementStreak(Guid userId)
    {
        var userData = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (userData == null) return ServiceResult<StreakUpdateResponse>.FailureResult("User data not found");

        DateTime today = DateTimeUtils.GetTodayLocalDate();
        DateTime yesterday = today.AddDays(-1);

        Streak? lastStreak = await _context.StreakRecords
            .Where(s => s.UserId == userData.Id)
            .OrderByDescending(s => s.ActivityDate)
            .FirstOrDefaultAsync();

        // Already counted today
        if (lastStreak?.ActivityDate == today)
            return ServiceResult<StreakUpdateResponse>.SuccessResult(new StreakUpdateResponse { UpdatedStreak=userData.CurrentStreak});

        // Continue streak
        if (lastStreak?.ActivityDate == yesterday)
        {
            userData.CurrentStreak += 1;
        }
        else
        {
            // New or broken streak
            userData.CurrentStreak = 1;
        }

        // Update longest streak
        if (userData.CurrentStreak > userData.LongestStreak)
        {
            userData.LongestStreak = userData.CurrentStreak;
        }

        // Record today's activity
        _context.StreakRecords.Add(new Streak
        {
            UserId = userData.Id,
            ActivityDate = today
        });

        await _context.SaveChangesAsync();
        return ServiceResult<StreakUpdateResponse>.SuccessResult(new StreakUpdateResponse { UpdatedStreak=userData.CurrentStreak });
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
