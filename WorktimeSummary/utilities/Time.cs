namespace WorktimeSummary.utilities
{
    using System;
    using System.Text.RegularExpressions;

    public class Time
    {
        public Time(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
            NormalizeTime();
        }

        public Time(string time)
        {
            // Regex for HH:MM(:SS.mmm)
            const string pattern = @"(?:[01]\d|2[0-3]):(?:[0-5]\d):?(?:[0-5]\d)?\.?\d?\d?\d?";

            Match m = Regex.Match(time, pattern);
            if (m.Length >= 8)
            {
                string[] values = time.Split(':');
                Hours = int.Parse(values[0]);
                Minutes = int.Parse(values[1]);
                if (values[2].Contains("."))
                {
                    Seconds = int.Parse(values[2].Split('.')[0]);
                    Milliseconds = int.Parse(values[2].Split('.')[1]);
                }
                else
                {
                    Seconds = int.Parse(values[2]);
                    Milliseconds = 0;
                }
            }
            else if (m.Length == 5)
            {
                string[] values = time.Split(':');
                Hours = int.Parse(values[0]);
                Minutes = int.Parse(values[1]);
                Seconds = 0;
                Milliseconds = 0;
            }
            else
            {
                throw new ArgumentException($"No valid time-string: {time}!");
            }
        }

        public int Milliseconds { get; set; }
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }

        public Time Add(Time other)
        {
            Hours += other.Hours;
            Minutes += other.Minutes;
            Seconds += other.Seconds;
            Milliseconds += other.Milliseconds;

            NormalizeTime();

            return this;
        }

        public Time AddMilliseconds(int milliseconds)
        {
            Milliseconds += milliseconds;
            NormalizeTime();
            return this;
        }

        public Time AddSeconds(int seconds)
        {
            Seconds += seconds;
            NormalizeTime();
            return this;
        }

        public Time AddMinutes(int minutes)
        {
            Minutes += minutes;
            NormalizeTime();
            return this;
        }

        public Time AddHours(int hours)
        {
            Hours += hours;
            return this;
        }

        public Time AddHours(float hours)
        {
            int hour = (int)hours;
            hours -= hour;
            int minutes = (int)(hours * 60f);
            hours -= minutes / 60f;
            int seconds = (int)(hours * 1000f);

            return AddSeconds(seconds).AddMinutes(minutes).AddHours(hour);
        }

        private void NormalizeTime()
        {
            int s = Seconds;
            Seconds += Milliseconds / 1000;
            Milliseconds -= (Seconds - s) * 1000;

            int m = Minutes;
            Minutes += Seconds / 60;
            Seconds -= (Minutes - m) * 60;

            int h = Hours;
            Hours += Minutes / 60;
            Minutes -= (Hours - h) * 60;
        }

        public override string ToString()
        {
            return Milliseconds > 0
                ? $"{Hours.ToString().PadLeft(2, '0')}" +
                  $":{Minutes.ToString().PadLeft(2, '0')}" +
                  $":{Seconds.ToString().PadLeft(2, '0')}" +
                  $".{Milliseconds.ToString().PadLeft(3, '0')}"
                : $"{Hours.ToString().PadLeft(2, '0')}" +
                  $":{Minutes.ToString().PadLeft(2, '0')}" +
                  $":{Seconds.ToString().PadLeft(2, '0')}";
        }

        public static Time Now()
        {
            DateTime now = DateTime.Now;
            return new Time(now.Hour, now.Minute, now.Second, now.Millisecond);
        }

        public DateTime ToDateTime()
        {
            return new DateTime(0, 0, 0, Hours, Minutes, Seconds, Milliseconds);
        }

        public static Time operator +(Time left, Time right)
        {
            return left.Add(right);
        }

        public static Time operator -(Time left, Time right)
        {
            left.Milliseconds -= right.Milliseconds;
            left.Seconds -= right.Seconds;
            left.Minutes -= right.Minutes;
            left.Hours -= right.Hours;

            if (left.Milliseconds < 0)
            {
                left.Milliseconds += 1000;
                left.Seconds -= 1;
            }

            if (left.Seconds < 0)
            {
                left.Seconds += 60;
                left.Minutes -= 1;
            }

            if (left.Minutes < 0)
            {
                left.Minutes += 60;
                left.Hours -= 1;
            }

            return left;
        }

        public double ToSeconds()
        {
            return HoursToSeconds(Hours) + MinutesToSeconds(Minutes) + Seconds + Milliseconds / 1000d;
        }

        public static float SecondsToMinutes(int seconds)
        {
            return seconds / 60f;
        }

        public static float MinutesToHours(float minutes)
        {
            return minutes / 60f;
        }

        public static float SecondsToHours(int seconds)
        {
            return seconds / 3600f;
        }

        public static float HoursToMinutes(float hours)
        {
            return hours * 60f;
        }

        public static float MinutesToSeconds(float minutes)
        {
            return minutes * 60f;
        }

        public static int HoursToSeconds(float hours)
        {
            return (int)(hours * 3600f);
        }
    }
}