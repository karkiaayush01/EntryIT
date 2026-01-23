using EntryIt.Common;
using EntryIt.Entities;

namespace EntryIt.Services;

public interface IMoodService
{
    Task<ServiceResult<List<Mood>>> GetMoods();
}
