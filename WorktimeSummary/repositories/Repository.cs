namespace WorktimeSummary.repositories
{
    using System;
    using System.IO;
    using data;
    using SQLite;

    public class Repository
    {
        protected static readonly SQLiteConnection Db =
            new SQLiteConnection(Path.Combine(Environment.CurrentDirectory, "data", "wt.db"));

        static Repository()
        {
            Db?.CreateTable<Worktimes>();
            Db?.CreateTable<UserSettings>();
            Db?.CreateTable<Issues>();
        }
    }
}