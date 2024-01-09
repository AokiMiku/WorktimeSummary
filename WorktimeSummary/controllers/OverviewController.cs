namespace WorktimeSummary.controllers
{
    using ApS;
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
            float dailyHoursToWork = userSettings.Settings.WorkhoursPerDay;
            double sumMinutesOt = 0;
            int sumDaysSick = 0;
            int sumDaysVacation = 0;
            for (int y = int.Parse(userSettings.Settings.StartingYear); y <= DateTime.Now.Year; y++)
            {
                List<Worktimes> days = repository.FindAllOfYear($"{y}");
                int daysSick = repository.CountSickDaysForYear($"{y}");
                int daysVacation = repository.CountVacationDaysForYear($"{y}");
                minutesOt += days
                    .Where(day =>
                        !day.IsVacation && !day.IsSickLeave &&
                        !DateSystem.IsPublicHoliday(day.Day.ToDateTime(), CountryCode.DE) &&
                        (!day.Day.Equals(DateTime.Today.ToCustomString()) ||
                         !userSettings.Settings.CurrentDayExcludedFromOvertimeCalculation))
                    .Sum(day => (day.Worktime - userSettings.Settings.CalculateBreakTime(day) - dailyHoursToWork) * 60d);

                overviewWindow.AddRow(new[]
                {
                    $"{y}", daysSick.ToString("0"), daysVacation.ToString("0"),
                    minutesOt.ToString("0.00", CultureInfo.CurrentCulture)
                });
                sumMinutesOt += minutesOt;
                sumDaysSick += daysSick;
                sumDaysVacation += daysVacation;
                minutesOt = 0;
            }
            overviewWindow.AddRow(new[] { "Sum", sumDaysSick.ToString("0"), sumDaysVacation.ToString("0"), sumMinutesOt.ToString("0.00") }, true);
        }
    }
}