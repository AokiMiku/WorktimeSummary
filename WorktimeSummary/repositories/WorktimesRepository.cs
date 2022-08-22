namespace WorktimeSummary.repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using data;
    using SQLite;

    public class WorktimesRepository : Repository
    {
        public WorktimesRepository()
        {
        }

        public Worktimes FindById(int id)
        {
            AsyncTableQuery<Worktimes> q = Db?.Table<Worktimes>().Where(w => w.Id == id);
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
            AsyncTableQuery<Worktimes> q = Db?.Table<Worktimes>();
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

        public List<Worktimes> FindAllByYearAndMonth(int year, int month)
        {
            return FindAllByYearAndMonth(year.ToString(), month.ToString());
        }

        public List<Worktimes> FindAllByYearAndMonth(string year, string month)
        {
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
            {
                return null;
            }

            string currentWhere = $"{year}-{month.PadLeft(2, '0')}";
            AsyncTableQuery<Worktimes> q = Db?.Table<Worktimes>()
                .Where(w => w.Day.StartsWith(currentWhere));
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