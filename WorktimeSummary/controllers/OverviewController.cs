namespace WorktimeSummary.controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using data;
    using Nager.Date;
    using repositories;
    using userSettings;
    using utilities;

    public class OverviewController
    {
        private readonly OverviewWindow overviewWindow;
        private readonly WorktimesRepository repository = WorktimesRepository.Instance;

        public OverviewController(OverviewWindow overviewWindow)
        {
            this.overviewWindow = overviewWindow;
            overviewWindow.AddHeader(new[]
            {
                "Year", "Days Sick", "Days On Vacation", "Minutes of Overtime"
            });
            FillData();
            this.overviewWindow.RepaintTable();
        }

        private void FillData()
        {
            double minutesOt = 0;
            float dailyHoursToWork = Settings.WorkhoursPerDay;
            for (int y = int.Parse(Settings.StartingYear); y <= DateTime.Now.Year; y++)
            {
                List<Worktimes> days = repository.FindAllOfYear($"{y}");
                int daysSick = repository.CountSickDaysForYear($"{y}");
                int daysVacation = repository.CountVacationDaysForYear($"{y}");
                minutesOt += days
                    .Where(day =>
                        !day.IsVacation && !day.IsSickLeave &&
                        !DateSystem.IsPublicHoliday(day.Day.ToDateTime(), CountryCode.DE) &&
                        (!day.Day.Equals(DateTime.Today.ToCustomString()) ||
                         !Settings.CurrentDayExcludedFromOvertimeCalculation))
                    .Sum(day => (day.Worktime - Settings.CalculateBreakTime(day) - dailyHoursToWork) * 60d);

                overviewWindow.AddRow(new[]
                {
                    $"{y}", daysSick.ToString(), daysVacation.ToString(),
                    minutesOt.ToString(CultureInfo.CurrentCulture)
                });
                minutesOt = 0;
            }
        }
    }
}