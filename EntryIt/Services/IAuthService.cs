using EntryIt.Common;
using EntryIt.Entities;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> Login(string email, string password);
    Task<ServiceResult<SignUpResponse>> SignUp(string fullname, string email, string username, string password);
    Task<ServiceResult<object?>> Logout();  //returns null hence object
    UserViewModel? GetCurrentUser();
}