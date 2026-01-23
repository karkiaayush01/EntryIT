using EntryIt.Common;
using EntryIt.Data;
using EntryIt.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntryIt.Services;

public class TagService: ITagService
{
    private readonly AppDbContext _context;
    
    public TagService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all the tags from the database
    /// </summary>
    /// <returns>List of all the tags in the database</returns>
    public async Task<ServiceResult<List<Tag>>> GetTags()
    {
        try
        {
            List<Tag> tags = await _context.Tags.ToListAsync();
            return ServiceResult<List<Tag>>.SuccessResult(tags);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<Tag>>.FailureResult($"Failed to get tags {ex.Message}");
        }
    }
}
