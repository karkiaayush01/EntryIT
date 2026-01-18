using EntryIt.Common;

public class AuthService : IAuthService
{
    public string CurrentUserId { get; set; } = string.Empty;
    public string CurrentUserEmail { get; set; } = string.Empty;

    public CurrentUser? GetCurrentUser()
    {
        if (CurrentUserId == string.Empty)
        {
            return null;
        }
        return new CurrentUser
        {
            UserId = CurrentUserId,
            Email = CurrentUserEmail
        };
    }

    public async Task<int> Awaitable()
    {
        await Task.Delay(3000);
        return 5;
    }
    public async Task<ServiceResult<LoginResponse>> Login(string email, string password)
    {
        int y = await Awaitable();
        return ServiceResult<LoginResponse>.FailureResult($"Error logging user in ");
    }

    public async Task<ServiceResult<SignUpResponse>> SignUp(string fullName, string email, string username, string password)
    {
        int y = await Awaitable();
        return ServiceResult<SignUpResponse>.FailureResult($"Error while creating account.");
    }
}