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
    /// <returns><see cref="ServiceResult{T}" />: List of all the <see cref="Tag"/> in the database</returns>
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

    /// <summary>
    /// Add a custom tag
    /// </summary>
    /// <param name="name">Name of the tag</param>
    /// <returns>An instance of <see cref="ServiceResult{T}"/>. The Success property of this instance will indicate whether the tag was successfully added.</returns>
    public async Task<ServiceResult<Tag>> AddCustomTag(string name)
    {
        try
        {
            Tag tag = new ()
            {
                Name = name,
                Type = "custom"
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return ServiceResult<Tag>.SuccessResult(tag);
        }
        catch (Exception ex)
        {
            return ServiceResult<Tag>.FailureResult($"Failed to add custom tag: {ex.Message}");
        }
    }
}
