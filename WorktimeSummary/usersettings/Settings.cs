namespace WorktimeSummary.userSettings
{
    using System;
    using System.Security.Cryptography;
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
            set
            {
                UserSettings us = new UserSettings
                {
                    SettingKeyMajor = "General",
                    SettingKeyMinor = "TableThemeTitle",
                    SettingValue = value
                };
                UserSettingsRepository.SaveSetting(us);
            }
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
            set
            {
                UserSettings us = new UserSettings
                {
                    SettingKeyMajor = "General",
                    SettingKeyMinor = "TableTheme1",
                    SettingValue = ((SolidColorBrush)value).Color.ToString()
                };
                UserSettingsRepository.SaveSetting(us);
            }
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
            set
            {
                UserSettings us = new UserSettings
                {
                    SettingKeyMajor = "General",
                    SettingKeyMinor = "TableTheme2",
                    SettingValue = ((SolidColorBrush)value).Color.ToString()
                };
                UserSettingsRepository.SaveSetting(us);
            }
        }

        public static string StartingYear
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "StartingYear");
                return us == null ? "0" : us.SettingValue;
            }
            set
            {
                UserSettings us = new UserSettings
                {
                    SettingKeyMajor = "General",
                    SettingKeyMinor = "StartingYear",
                    SettingValue = value
                };
                UserSettingsRepository.SaveSetting(us);
            }
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