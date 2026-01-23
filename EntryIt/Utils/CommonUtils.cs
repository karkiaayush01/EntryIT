namespace EntryIt.Utils;

public class CommonUtils
{
    /// <summary>
    /// Get the initials from the full name of the user
    /// </summary>
    /// <param name="fullname">The full name of the user</param>
    /// <returns>The Initials of the user</returns>
    public static string GetInitials(string fullname)
    {
        if (string.IsNullOrWhiteSpace(fullname))
            return string.Empty;

        // Split by spaces
        var parts = fullname.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Take first character of each part and make uppercase
        var initials = string.Concat(parts.Select(p => char.ToUpper(p[0])));

        return initials;
    }

    /// <summary>
    /// Get the total word count from a provided document content
    /// </summary>
    /// <param name="content">The text content from which to extract word count from</param>
    /// <returns>The total word count of the content</returns>
    public static int GetWordCount(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return 0;

        // Split on whitespace characters (space, tab, newline)
        var words = content
            .Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return words.Length;
    }
}
