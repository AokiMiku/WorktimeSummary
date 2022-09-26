namespace WorktimeSummary.userSettings
{
    using System.Globalization;
    using System.Windows.Media;
    using data;
    using repositories;

    public static class Settings
    {
        private static readonly UserSettingsRepository UserSettingsRepository;

        static Settings()
        {
            UserSettingsRepository = new UserSettingsRepository();
        }

        public static string TableThemeTitle
        {
            get => Get("General", "TableThemeTitle");
            set => Save("General", "TableThemeTitle", value);
        }

        public static Brush TableTheme1
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "TableTheme1");
                if (us == null)
                {
                    return Brushes.White;
                }

                BrushConverter converter = new BrushConverter();
                return (Brush)converter.ConvertFromString(us.SettingValue);
            }
            set => Save("General", "TableTheme1", ((SolidColorBrush)value).Color.ToString());
        }

        public static Brush TableTheme2
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "TableTheme2");
                if (us == null)
                {
                    return Brushes.Beige;
                }

                BrushConverter converter = new BrushConverter();
                return (Brush)converter.ConvertFromString(us.SettingValue);
            }
            set => Save("General", "TableTheme2", ((SolidColorBrush)value).Color.ToString());
        }

        public static string StartingYear
        {
            get => Get("General", "StartingYear");
            set => Save("General", "StartingYear", value);
        }

        public static float WorkhoursPerWeek
        {
            get => float.Parse(Get("General", "WorkhoursPerWeek"));
            set => Save("General", "WorkhoursPerWeek", value.ToString(CultureInfo.CurrentCulture));
        }

        public static bool ShowWeekends
        {
            get => bool.Parse(Get("General", "ShowWeekends"));
            set => Save("General", "ShowWeekends", value.ToString(CultureInfo.CurrentCulture));
        }

        public static bool CurrentDayBold
        {
            get => bool.Parse(Get("General", "CurrentDayBold"));
            set => Save("General", "CurrentDayBold", value.ToString());
        }

        public static bool AutoRefreshEnabled
        {
            get => bool.Parse(Get("Schedules", "AutoRefreshEnabled"));
            set => Save("Schedules", "AutoRefreshEnabled", value.ToString());
        }

        public static int AutoRefreshEveryXMinutes
        {
            get => int.Parse(Get("Schedules", "AutoRefreshEveryXMinutes"));
            set => Save("Schedules", "AutoRefreshEveryXMinutes", value.ToString());
        }

        public static int AutoSaveEveryXMinutes
        {
            get => int.Parse(Get("Schedules", "AutoSaveEveryXMinutes"));
            set => Save("Schedules", "AutoSaveEveryXMinutes", value.ToString());
        }

        private static string Get(string majorKey, string minorKey)
        {
            UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey(majorKey, minorKey);
            return us == null ? "" : us.SettingValue;
        }

        private static void Save(string majorKey, string minorKey, string value)
        {
            UserSettings us = new UserSettings
            {
                SettingKeyMajor = $"{majorKey}",
                SettingKeyMinor = $"{minorKey}",
                SettingValue = value
            };
            UserSettingsRepository.SaveSetting(us);
        }


        public static bool IsLeapYear(int year)
        {
            if (year % 4 == 0 && year % 100 != 0)
            {
                return true;
            }

            if (year % 100 == 0)
            {
                return year % 400 == 0;
            }

            return false;
        }
    }
}