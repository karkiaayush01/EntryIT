using EntryIt.Services;
using EntryIt.Models;
using EntryIt.Common;

public class AuthService : IAuthService
{
    public string CurrentUserId { get; set; } = string.Empty;
    public string CurrentUserEmail { get; set; } = string.Empty;

    public async Task<int> Awaitable()
    {
        return 5;
    }
    public async Task<ServiceResult<LoginResponse>> Login(string email, string password)
    {
        int y = await Awaitable();
        return ServiceResult<LoginResponse>.FailureResult($"Error logging user in ");
    }

    public async Task<ServiceResult<SignUpResponse>> SignUp(string email, string fullName, string birthdate, string username, string password)
    {
        int y = await Awaitable();
        return ServiceResult<SignUpResponse>.FailureResult($"Error while creating account.");
    }
}