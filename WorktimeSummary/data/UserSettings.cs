namespace WorktimeSummary.data
{
    using SQLite;

    [Table("userSettings")]
    public class UserSettings
    {
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("settingKeyMajor")] public string SettingKeyMajor { get; set; }

        [Column("settingKeyMinor")] public string SettingKeyMinor { get; set; }

        [Column("settingValue")] public string SettingValue { get; set; }
    }
}