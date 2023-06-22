namespace WorktimeSummary.repositories
{
    using System;
    using System.IO;
    using WorktimeSummary.userSettings;
    using data;
    using SQLite;

    public class Repository
    {
        protected static readonly SQLiteConnection Db =
            new SQLiteConnection(Settings.DatabasePath);

        static Repository()
        {
            Db?.CreateTable<Worktimes>();
            Db?.CreateTable<UserSettings>();
            Db?.CreateTable<Issues>();
        }
    }
}