namespace WorktimeSummary.data
{
    using System;
    using essentials;
    using SQLite;

    [Table("worktimes")]
    public class Worktimes
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("day")] public string Day { get; set; }

        [Column("pause")] public int Pause { get; set; }

        [Column("worktime")] public double Worktime { get; set; }

        [Column("is_workday")] public bool IsWorkday { get; set; }

        [Column("starting_time")] public string StartingTimeString { get; set; }

        [Column("is_sick_leave")] public bool IsSickLeave { get; set; }

        [Column("is_vacation")] public bool IsVacation { get; set; }

        [Ignore]
        public Time StartingTime
        {
            get => new Time(StartingTimeString);
            set => StartingTimeString = value.ToString();
        }

        public static Worktimes DefaultData(string day)
        {
            return new Worktimes
            {
                Day = day,
                StartingTime = new Time(),
                IsSickLeave = false,
                IsVacation = false,
                Pause = 0,
                Worktime = 0
            };
        }
    }
}