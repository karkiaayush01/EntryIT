
namespace EntryIt.Utils;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
public class ValidationUtils
{
    public static bool IsBlank (string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    // Overriding IsBlank to check multiple strings
    public static bool IsBlank(List<string> values)
    {
        if (values == null || values.Count == 0)
            return true; // treat null/empty list as blank

        // Returns true if **any string is blank**
        return values.Any(string.IsNullOrWhiteSpace);
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(email);
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return false;

        // At least one letter, one number, one symbol
        Regex PasswordRegex = new Regex(
            @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{6,}$",
            RegexOptions.Compiled
        );

        return PasswordRegex.IsMatch(password);
    }
}