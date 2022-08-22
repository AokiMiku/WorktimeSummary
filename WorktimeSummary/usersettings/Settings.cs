namespace WorktimeSummary.userSettings
{
    using System.Windows.Media;
    using data;
    using repositories;

    public static class Settings
    {
        private static UserSettingsRepository userSettingsRepository;

        static Settings()
        {
            userSettingsRepository = new UserSettingsRepository();
        }

        public static string TableThemeTitle
        {
            get
            {
                UserSettings us = userSettingsRepository.FindByMajorAndMinorKey("General", "TableThemeTitle");
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
                userSettingsRepository.SaveSetting(us);
            }
        }
        
        public static Brush TableTheme1
        {
            get
            {
                UserSettings us = userSettingsRepository.FindByMajorAndMinorKey("General", "TableTheme1");
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
                userSettingsRepository.SaveSetting(us);
            }
        }
        
        public static Brush TableTheme2
        {
            get
            {
                UserSettings us = userSettingsRepository.FindByMajorAndMinorKey("General", "TableTheme2");
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
                userSettingsRepository.SaveSetting(us);
            }
        }
    }
}