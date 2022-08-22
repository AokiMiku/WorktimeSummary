namespace WorktimeSummary.repositories
{
    using System;
    using System.IO;
    using data;
    using SQLite;

    public class Repository
    {
        protected static readonly SQLiteAsyncConnection Db =
            new SQLiteAsyncConnection(Path.Combine(Environment.CurrentDirectory, "data", "wt.db"));

        static Repository()
        {
            Db?.CreateTableAsync<Worktimes>();
            Db?.CreateTableAsync<UserSettings>();
        }
    }
}