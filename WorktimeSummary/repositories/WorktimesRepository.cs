namespace WorktimeSummary.repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using data;
    using SQLite;
    using utilities;

    public class WorktimesRepository : Repository
    {
        private WorktimesRepository()
        {
        }

        public static WorktimesRepository Instance { get; } = new WorktimesRepository();

        public Worktimes FindById(int id)
        {
            TableQuery<Worktimes> q = Db?.Table<Worktimes>().Where(w => w.Id == id);
            List<Worktimes> r = q?.ToList();
            try
            {
                return r?.First();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<Worktimes> FindAll()
        {
            TableQuery<Worktimes> q = Db?.Table<Worktimes>();
            List<Worktimes> r = q?.ToList();
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

        public List<Worktimes> FindAllOfYear(string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return null;
            }

            TableQuery<Worktimes> q = Db?.Table<Worktimes>().Where(w => w.Day.StartsWith(year));
            List<Worktimes> r = q?.ToList();
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

        public int CountSickDaysForYear(string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return 0;
            }

            int? q = Db?.Table<Worktimes>()?.Count(w => w.Day.StartsWith(year) && w.IsSickLeave);
            try
            {
                if (q != null)
                {
                    return (int)q;
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                throw;
            }

            return 0;
        }

        public int CountVacationDaysForYear(string year)
        {
            if (string.IsNullOrEmpty(year))
            {
                return 0;
            }

            int? q = Db?.Table<Worktimes>()?.Count(w => w.Day.StartsWith(year) && w.IsVacation);
            try
            {
                if (q != null)
                {
                    return (int)q;
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                throw;
            }

            return 0;
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
            TableQuery<Worktimes> q = Db?.Table<Worktimes>()
                .Where(w => w.Day.StartsWith(currentWhere));
            List<Worktimes> r = q?.ToList();
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

        public Worktimes FindToday()
        {
            DateTime today = DateTime.Today;
            return FindByDay(today.ToCustomString());
        }

        public Worktimes FindByDay(string day)
        {
            TableQuery<Worktimes> q = Db?.Table<Worktimes>().Where(w => w.Day.Equals(day));
            List<Worktimes> r = q?.ToList();
            try
            {
                return r?.First();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                return Worktimes.DefaultData(day);
            }
        }

        public void Save(Worktimes worktimes)
        {
            if ((FindByDay(worktimes.Day) == null) || (FindByDay(worktimes.Day).Id == 0))
            {
                Db?.Insert(worktimes);
            }
            else
            {
                worktimes.Id = FindByDay(worktimes.Day).Id;
                int? updateAsync = Db?.Update(worktimes);
                if (updateAsync > 0)
                {
                    Console.Out.WriteLine("Success");
                }
            }
        }
    }
}