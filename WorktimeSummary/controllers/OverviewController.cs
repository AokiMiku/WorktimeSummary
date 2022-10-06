namespace WorktimeSummary.controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using data;
    using Nager.Date;
    using repositories;
    using userSettings;
    using utilities;

    public class OverviewController
    {
        private readonly WorktimesRepository repository = WorktimesRepository.Instance;
        private readonly OverviewWindow overviewWindow;

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
            List<Worktimes> days = repository.FindAll();
            double minutesOt = 0;
            string currentYear = "y";
            float dailyHoursToWork = Settings.WorkhoursPerWeek / 5;
            for (int i = 0; i < days.Count; i++)
            {
                if (days[i].Day.StartsWith(currentYear))
                {
                    continue;
                }

                currentYear = days[i].Day.Substring(0, 4);
                int daysSick = repository.CountSickDaysForYear(currentYear);
                int daysVacation = repository.CountVacationDaysForYear(currentYear);
                int index = i;
                while (i < days.Count && days[i].Day.StartsWith(currentYear))
                {
                    if (!days[i].IsVacation && !days[i].IsSickLeave &&
                        !DateSystem.IsPublicHoliday(days[i].Day.ToDateTime(), CountryCode.DE))
                    {
                        minutesOt += (days[i].Worktime - days[i].Pause / 3600d - dailyHoursToWork) * 60d;
                    }

                    i++;
                }

                i = index;
                overviewWindow.AddRow(new[]
                {
                    currentYear, daysSick.ToString(), daysVacation.ToString(),
                    minutesOt.ToString(CultureInfo.CurrentCulture)
                });
                minutesOt = 0;
            }
        }
    }
}