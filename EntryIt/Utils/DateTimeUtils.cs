namespace EntryIt.Utils;

public class DateTimeUtils
{
    public static DateTime GetTodayLocalDate()
    {
        return DateTime.Now.Date;
    }

    public static DateTime GetUtcDateTime()
    {
        return DateTime.UtcNow;
    }
}
