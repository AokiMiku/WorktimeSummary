namespace WorktimeSummary.repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using data;
    using SQLite;

    public class UserSettingsRepository : Repository
    {
        public UserSettings FindById(int id)
        {
            AsyncTableQuery<UserSettings> q = Db?.Table<UserSettings>().Where(us => us.Id == id);
            Task<UserSettings> r = q?.ToListAsync().ContinueWith(us => us.Result.First());
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

        public UserSettings FindByMajorAndMinorKey(string majorKey, string minorKey)
        {
            AsyncTableQuery<UserSettings> q = Db?.Table<UserSettings>()
                .Where(us => us.SettingKeyMajor == majorKey && us.SettingKeyMinor == minorKey);
            Task<UserSettings> r = null;

            r = q?.ToListAsync().ContinueWith(us => us.Result.FirstOrDefault());

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

        public void SaveSetting(UserSettings userSettings)
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