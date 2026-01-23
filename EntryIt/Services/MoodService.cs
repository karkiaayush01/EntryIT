using EntryIt.Common;
using EntryIt.Data;
using EntryIt.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntryIt.Services;

public class MoodService : IMoodService
{
    private readonly AppDbContext _context;

    public MoodService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<Mood>>> GetMoods()
    {
        try
        {
            List<Mood> Moods = await _context.Moods.ToListAsync();
            return ServiceResult<List<Mood>>.SuccessResult(Moods);
        }
        catch(Exception ex)
        {
            return ServiceResult<List<Mood>>.FailureResult($"Failed to get moods. {ex.Message}");
        }
    }
}
