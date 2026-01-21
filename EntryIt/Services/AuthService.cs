using EntryIt.Common;
using EntryIt.Data;
using Microsoft.EntityFrameworkCore;
using EntryIt.Entities;

public class AuthService : IAuthService
{
    public UserViewModel? CurrentUser { get; set; }
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    private UserViewModel MapUserViewModel(User user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Username = user.Username
        };
    }

    public UserViewModel? GetCurrentUser()
    {
        if (CurrentUser == null)
        {
            return null;
        }
        return CurrentUser;
    }

    public async Task<ServiceResult<LoginResponse>> Login(string identifier, string password)
    {
        try
        {
            await Task.Delay(2000); //Simulate API
            var userData = await _context.Users.
                FirstOrDefaultAsync(
                    u => u.Email == identifier || u.Username == identifier
                );

            if (userData == null)
            {
                return ServiceResult<LoginResponse>.FailureResult($"Could not find your account");
            }
            else
            {
                string userPasswordHash = userData.Password;
                bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, userPasswordHash);

                if (passwordMatch)
                {
                    UserViewModel user = MapUserViewModel(userData);
                    CurrentUser = user;
                    return ServiceResult<LoginResponse>.SuccessResult(new LoginResponse
                    {
                        User = user
                    });
                }
                else
                {
                    return ServiceResult<LoginResponse>.FailureResult($"Invalid username or password.");
                }
            }

        }
        catch (Exception ex)
        {
            return ServiceResult<LoginResponse>.FailureResult($"Error logging user in {ex.Message}");
        }
    }

    public async Task<ServiceResult<SignUpResponse>> SignUp(string fullName, string email, string username, string password)
    {
        await Task.Delay(2000); //Simulate API
        try
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var existingUserByEmail = await _context.Users.
                FirstOrDefaultAsync(
                    u => u.Email == email
                );

            var existingUserByUsername = await _context.Users.
                FirstOrDefaultAsync(
                    u => u.Username == username
                );

            if (existingUserByEmail != null)
            {
                return ServiceResult<SignUpResponse>.FailureResult("Email has already been used");
            }
            else if (existingUserByUsername != null)
            {
                return ServiceResult<SignUpResponse>.FailureResult("Username has already been taken");
            }
            else
            {
                var user = new User
                {
                    FullName = fullName,
                    Email = email,
                    Username = username,
                    Password = hashedPassword
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var response = new SignUpResponse {
                    UserId = user.Id
                };

                return ServiceResult<SignUpResponse>.SuccessResult(response);
            }
        }
        catch (Exception ex)
        {
            return ServiceResult<SignUpResponse>.FailureResult($"Error while creating account. {ex.Message}");
        }
    }

    public async Task<ServiceResult<object?>> Logout()
    {
        try
        {
            await Task.Delay(1000);
            CurrentUser = null;
            return ServiceResult<object?>.SuccessResult(null);
        }
        catch(Exception ex)
        {
            return ServiceResult<object?>.FailureResult($"Error while logging user out {ex.Message}");
        }
    }
}