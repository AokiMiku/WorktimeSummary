namespace WorktimeSummary.userSettings
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;
    using data;
    using repositories;
    using utilities;

    public static class Settings
    {
        private static readonly UserSettingsRepository UserSettingsRepository;

        static Settings()
        {
            UserSettingsRepository = new UserSettingsRepository();

            if (DatabasePath.Length < 5)
            {
                DatabasePath = Path.Combine(Environment.CurrentDirectory, "data", "wt.db");
            }
        }

        public static string TableThemeTitle
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "TableThemeTitle");
                if (us != null)
                {
                    return us.SettingValue;
                }

                TableThemeTitle = "Gray";
                return TableThemeTitle;
            }
            set => Save("General", "TableThemeTitle", value);
        }

        public static Brush TableTheme1
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "TableTheme1");
                if (us != null)
                {
                    return (Brush)new BrushConverter().ConvertFromString(us.SettingValue);
                }

                TableTheme1 = (Brush)new BrushConverter().ConvertFromString("#9F9F9F");
                return TableTheme1;
            }
            set => Save("General", "TableTheme1", ((SolidColorBrush)value).Color.ToString());
        }

        public static Brush TableTheme2
        {
            get
            {
                UserSettings us = UserSettingsRepository.FindByMajorAndMinorKey("General", "TableTheme2");
                if (us != null)
                {
                    return (Brush)new BrushConverter().ConvertFromString(us.SettingValue);
                }

                TableTheme2 = (Brush)new BrushConverter().ConvertFromString("#C6C6C6");
                return TableTheme2;
            }
            set => Save("General", "TableTheme2", ((SolidColorBrush)value).Color.ToString());
        }

        public static string StartingYear
        {
            get => Get("General", "StartingYear");
            set => Save("General", "StartingYear", value);
        }

        public static int WorkdaysPerWeek
        {
            get => GetAsInt("General", "WorkdaysPerWeek");
            set => Save("General", "WorkdaysPerWeek", value.ToString());
        }

        public static float WorkhoursPerWeek
        {
            get => GetAsFloat("General", "WorkhoursPerWeek");
            set => Save("General", "WorkhoursPerWeek", value.ToString(CultureInfo.CurrentCulture));
        }

        public static float WorkhoursPerDay
        {
            get
            {
                float hours = WorkhoursPerWeek / WorkdaysPerWeek;
                if (hours > 9)
                {
                    hours += 0.75f;
                }
                else if (hours > 6)
                {
                    hours += 0.5f;
                }

                return hours;
            }
        }

        public static bool ShowWeekends
        {
            get => GetAsBool("General", "ShowWeekends");
            set => Save("General", "ShowWeekends", value.ToString(CultureInfo.CurrentCulture));
        }

        public static bool CurrentDayBold
        {
            get => GetAsBool("General", "CurrentDayBold");
            set => Save("General", "CurrentDayBold", value.ToString());
        }

        public static bool CurrentDayExcludedFromOvertimeCalculation
        {
            get => GetAsBool("General", "CurrentDayExcludedFromOvertimeCalculation");
            set => Save("General", "CurrentDayExcludedFromOvertimeCalculation", value.ToString());
        }

        public static bool WeeklySummaries
        {
            get => GetAsBool("General", "WeeklySummaries");
            set => Save("General", "WeeklySummaries", value.ToString());
        }

        public static bool AutoRefreshEnabled
        {
            get => GetAsBool("Schedules", "AutoRefreshEnabled");
            set => Save("Schedules", "AutoRefreshEnabled", value.ToString());
        }

        public static int AutoRefreshEveryXMinutes
        {
            get => GetAsInt("Schedules", "AutoRefreshEveryXMinutes");
            set => Save("Schedules", "AutoRefreshEveryXMinutes", value.ToString());
        }

        public static int AutoSaveEveryXMinutes
        {
            get => GetAsInt("Schedules", "AutoSaveEveryXMinutes");
            set => Save("Schedules", "AutoSaveEveryXMinutes", value.ToString());
        }

        public static bool AutoUpdate
        {
            get => GetAsBool("Updates", "AutoUpdate");
            set => Save("Updates", "AutoUpdate", value.ToString());
        }
        
        public static DateTime LastUpdate
        {
            get => GetAsDateTime("Updates", "LastUpdate");
            set => Save("Updates", "LastUpdate", value.ToString(CultureInfo.CurrentCulture));
        }

        public static string DatabasePath
        {
            get => ConfigurationManager.ConnectionStrings["SQLite"].ConnectionString.Replace("Data Source=", "").Replace(";", "").Trim();

            set
            {
                // Get the connection string settings
                ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["SQLite"];

                // Update the connection string with the user input
                string updatedConnectionString = string.Format("Data Source={0};", value);

                // Create a new connection string settings object with the updated connection string
                ConnectionStringSettings updatedSettings = new ConnectionStringSettings(connectionStringSettings.Name,
                    updatedConnectionString, connectionStringSettings.ProviderName);

                // Update the connection string in the configuration file
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.ConnectionStrings.ConnectionStrings.Remove(connectionStringSettings.Name);
                configuration.ConnectionStrings.ConnectionStrings.Add(updatedSettings);
                configuration.Save(ConfigurationSaveMode.Modified);

                // Refresh the configuration manager
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }

        private static DateTime GetAsDateTime(string majorKey, string minorKey)
        {
            string get = Get(majorKey, minorKey);
            return get.Length > 0 ? DateTime.Parse(get) : DateTime.MinValue;
        }

        private static float GetAsFloat(string majorKey, string minorKey)
        {
            string get = Get(majorKey, minorKey);
            if (string.IsNullOrEmpty(get))
            {
                return 0;
            }

            try
            {
                return float.Parse(get);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        private static int GetAsInt(string majorKey, string minorKey)
        {
            return (int)GetAsFloat(majorKey, minorKey);
        }

        private static bool GetAsBool(string majorKey, string minorKey)
        {
            string get = Get(majorKey, minorKey);
            if (string.IsNullOrEmpty(get))
            {
                return false;
            }

            try
            {
                return bool.Parse(get);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
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
            if (((year % 4) == 0) && ((year % 100) != 0))
            {
                return true;
            }

            if ((year % 100) == 0)
            {
                return (year % 400) == 0;
            }

            return false;
        }

        public static double CalculateBreakTime(Worktimes wt)
        {
            return WorkhoursPerDay > 9 ? Time.SecondsToHours(wt.Pause - (int)Time.MinutesToSeconds(45)) :
                WorkhoursPerDay > 6 ? Time.SecondsToHours(wt.Pause - (int)Time.MinutesToSeconds(30)) :
                Time.SecondsToHours(wt.Pause);
        }
    }
}