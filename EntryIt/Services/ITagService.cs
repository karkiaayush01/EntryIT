namespace EntryIt.Services;

using EntryIt.Common;
using EntryIt.Entities;

public interface ITagService
{
    Task<ServiceResult<List<Tag>>> GetTags();
}
