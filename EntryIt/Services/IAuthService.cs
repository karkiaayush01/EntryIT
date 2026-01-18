using EntryIt.Common;
using EntryIt.Models;
using EntryIt.Services;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> Login(string email, string password);
    Task<ServiceResult<SignUpResponse>> SignUp(string email, string fullName, string birthdate, string username, string password);
}