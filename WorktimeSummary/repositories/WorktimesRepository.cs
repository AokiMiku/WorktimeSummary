namespace WorktimeSummary.repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using data;
    using SQLite;

    public class WorktimesRepository : Repository
    {
        private WorktimesRepository()
        {
        }

        public static WorktimesRepository Instance { get; } = new WorktimesRepository();

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

        public Worktimes FindToday()
        {
            DateTime today = DateTime.Today;
            return FindByDay($"{today.Year:0000}-{today.Month:00}-{today.Day:00}");
        }

        public Worktimes FindByDay(string day)
        {
            AsyncTableQuery<Worktimes> q = Db?.Table<Worktimes>().Where(w => w.Day.Equals(day));
            Task<Worktimes> r = q?.ToListAsync().ContinueWith(w => w.Result.First());
            try
            {
                return r?.Result;
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
                return Worktimes.DefaultData(day);
            }
        }

        public void Save(Worktimes worktimes)
        {
            if (FindByDay(worktimes.Day) == null || FindByDay(worktimes.Day).Id == 0)
            {
                Db?.InsertAsync(worktimes);
            }
            else
            {
                worktimes.Id = FindByDay(worktimes.Day).Id;
                Task<int> updateAsync = Db?.UpdateAsync(worktimes);
                if (updateAsync?.Result > 0)
                {
                    Console.Out.WriteLine("Success");
                }
            }
        }
    }
}