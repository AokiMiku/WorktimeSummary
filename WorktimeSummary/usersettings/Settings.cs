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
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "TableThemeTitle");
                return us == null ? "" : us.SettingValue;
            }
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
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "StartingYear");
                return us == null ? "0" : us.SettingValue;
            }
            set => Save("General", "StartingYear", value);
        }

        public static float WorkhoursPerWeek
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "WorkhoursPerWeek");
                return us == null ? 0 : float.Parse(us.SettingValue);
            }
            set => Save("General", "WorkhoursPerWeek", value.ToString(CultureInfo.CurrentCulture));
        }

        public static bool ShowWeekends
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "ShowWeekends");
                return us != null && bool.Parse(us.SettingValue);
            }
            set => Save("General", "ShowWeekends", value.ToString(CultureInfo.CurrentCulture));
        }
        
        public static bool CurrentDayBold
        {
            get
            {
                UserSettings us =
                    UserSettingsRepository.FindByMajorAndMinorKey("General", "CurrentDayBold");
                return us != null && bool.Parse(us.SettingValue);
            }
            set => Save("General", "CurrentDayBold", value.ToString());
        }

        public static bool AutoRefreshEnabled
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("Schedules", "AutoRefreshEnabled");
                return us != null && bool.Parse(us.SettingValue);
            }
            set => Save("Schedules", "AutoRefreshEnabled", value.ToString());
        }

        public static int AutoRefreshEveryXMinutes
        {
            get
            {
                UserSettings us =
                    UserSettingsRepository.FindByMajorAndMinorKey("Schedules", "AutoRefreshEveryXMinutes");
                return us == null ? 0 : int.Parse(us.SettingValue);
            }
            set => Save("Schedules", "AutoRefreshEveryXMinutes", value.ToString());
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