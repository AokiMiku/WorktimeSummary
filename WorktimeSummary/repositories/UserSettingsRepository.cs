namespace WorktimeSummary.repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using SQLite;

    public class UserSettingsRepository : Repository
    {
        public UserSettingsRepository()
        {
        }

        public data.UserSettings FindById(int id)
        {
            AsyncTableQuery<data.UserSettings> q = Db?.Table<data.UserSettings>().Where(us => us.Id == id);
            Task<data.UserSettings> r = q?.ToListAsync().ContinueWith(us => us.Result.First());
            try
            {
                return r?.Result;
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public data.UserSettings FindByMajorAndMinorKey(string majorKey, string minorKey)
        {
            AsyncTableQuery<data.UserSettings> q = Db?.Table<data.UserSettings>()
                .Where(us => us.SettingKeyMajor == majorKey && us.SettingKeyMinor == minorKey);
            Task<data.UserSettings> r = q?.ToListAsync().ContinueWith(us => us.Result.First());
            try
            {
                return r?.Result;
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void SaveSetting(data.UserSettings userSettings)
        {
            if (FindByMajorAndMinorKey(userSettings.SettingKeyMajor, userSettings.SettingKeyMinor) == null)
            {
                Db?.InsertAsync(userSettings);
            }
            else
            {
                userSettings.Id = FindByMajorAndMinorKey(userSettings.SettingKeyMajor, userSettings.SettingKeyMinor).Id;
                Db?.UpdateAsync(userSettings);
            }
        }
    }
}