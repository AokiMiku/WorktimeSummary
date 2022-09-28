namespace WorktimeSummary.utilities
{
    using System;

    public static class Extensions
    {
        public static DateTime ToDateTime(this string s)
        {
            return DateTime.Parse(s);
        }
    }
}