namespace WorktimeSummary.repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using data;
    using SQLite;

    public class UserSettingsRepository : Repository
    {
        public UserSettings FindById(int id)
        {
            TableQuery<UserSettings> q = Db?.Table<UserSettings>().Where(us => us.Id == id);
            List<UserSettings> r = q?.ToList();
            try
            {
                return r.First();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public UserSettings FindByMajorAndMinorKey(string majorKey, string minorKey)
        {
            TableQuery<UserSettings> q = Db?.Table<UserSettings>()
                .Where(us => (us.SettingKeyMajor == majorKey) && (us.SettingKeyMinor == minorKey));
            UserSettings r = q?.ToList().FirstOrDefault();

            try
            {
                return r;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void SaveSetting(UserSettings userSettings)
        {
            if (FindByMajorAndMinorKey(userSettings.SettingKeyMajor, userSettings.SettingKeyMinor) == null)
            {
                Db?.Insert(userSettings);
            }
            else
            {
                userSettings.Id = FindByMajorAndMinorKey(userSettings.SettingKeyMajor, userSettings.SettingKeyMinor).Id;
                Db?.Update(userSettings);
            }
        }
    }
}