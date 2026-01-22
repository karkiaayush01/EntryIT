using EntryIt.Models;
using EntryIt.Common;

namespace EntryIt.Services;

public interface IStreakService
{
    public Task<ServiceResult<StreakUpdateResponse>> IncrementStreak(Guid userId);
    public Task<ServiceResult<StreakUpdateResponse>> DecrementStreak(Guid userId);
}
