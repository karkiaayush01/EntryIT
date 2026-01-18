using EntryIt.Common;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> Login(string email, string password);
    Task<ServiceResult<SignUpResponse>> SignUp(string fullname, string email, string username, string password);
    CurrentUser? GetCurrentUser();
}