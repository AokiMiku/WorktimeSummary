namespace WorktimeSummary.utilities
{
    using System;

    public static class Extensions
    {
        public static string ToCustomString(this DateTime d)
        {
            return $"{d.Year:0000}-{d.Month:00}-{d.Day:00}";
        }
    }
}