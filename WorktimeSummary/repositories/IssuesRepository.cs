namespace WorktimeSummary.repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using data;
    using SQLite;

    public class IssuesRepository : Repository
    {
        private IssuesRepository()
        {
        }

        public static IssuesRepository Instance { get; set; } = new IssuesRepository();

        public Issues FindByIssueNumber(string issueNumber)
        {
            TableQuery<Issues> q = Db?.Table<Issues>().Where(w => w.IssueNumber == issueNumber);
            List<Issues> r = q?.ToList();
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

        public int FindFunctionPointsByIssueNumber(string issueNumber)
        {
            return FindByIssueNumber(issueNumber).FunctionPoints;
        }

        public void Save(Issues issues)
        {
            if (FindByIssueNumber(issues.IssueNumber) == null)
            {
                Db?.Insert(issues);
            }
            else
            {
                issues.IssueNumber = FindByIssueNumber(issues.IssueNumber).IssueNumber;
                int? updateAsync = Db?.Update(issues);
                if (updateAsync > 0)
                {
                    Console.Out.WriteLine("Success");
                }
            }
        }
    }
}