namespace WorktimeSummary.essentials
{
    using System;
    using System.Text.RegularExpressions;

    public class Time
    {
        public int Milliseconds { get; set; }
        public int Seconds { get; set; }
        public int Minutes { get; set; }
        public int Hours { get; set; }


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
    }
}