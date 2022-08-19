namespace WorktimeSummary.data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using SQLite;

    public class WorktimesRepository
    {
        private readonly SQLiteAsyncConnection db =
            new SQLiteAsyncConnection(Path.Combine(Environment.CurrentDirectory, "data", "wt.db"));

        public WorktimesRepository()
        {
            db?.CreateTableAsync<Worktimes>();
            db?.CreateTableAsync<UserSettings>();
        }

        public Worktimes FindById(int id)
        {
            AsyncTableQuery<Worktimes> q = db?.Table<Worktimes>().Where(w => w.Id == id);
            Task<Worktimes> r = q?.ToListAsync().ContinueWith(w => w.Result.First());
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

        public List<Worktimes> FindAll()
        {
            AsyncTableQuery<Worktimes> q = db?.Table<Worktimes>();
            Task<List<Worktimes>> r = q?.ToListAsync().ContinueWith(w => w.Result);
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
    }
}