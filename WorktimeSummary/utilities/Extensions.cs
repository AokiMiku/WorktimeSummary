namespace WorktimeSummary.utilities
{
    using System;

    public static class Extensions
    {
        public static DateTime ToDateTime(this string s)
        {
            return DateTime.Parse(s);
        }

        public static string ToCustomString(this DateTime d)
        {
            return $"{d.Year:0000}-{d.Month:00}-{d.Day:00}";
        }
    }
}