namespace WorktimeSummary.data
{
    using SQLite;

    [Table("issues")]
    public class Issues
    {
        [PrimaryKey] [Column("issueNumber")] public string IssueNumber { get; set; }

        [Column("seconds")] public int Seconds { get; set; }

        [Column("FunctionPoints")] public int FunctionPoints { get; set; }
    }
}