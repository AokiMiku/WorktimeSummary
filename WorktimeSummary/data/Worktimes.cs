namespace WorktimeSummary.data
{
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

        public Time StartingTime
        {
            get => new Time(StartingTimeString);
            set => StartingTimeString = value.ToString();
        }
    }
}