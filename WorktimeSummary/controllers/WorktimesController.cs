namespace WorktimeSummary.controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using ApS;
    using data;
    using Nager.Date;
    using repositories;
    using userSettings;
    using utilities;

    public class WorktimesController
    {
        private const int IndexSickLeave = 6;
        private const int IndexVacation = 7;
        private const string Format = "0.##";
        private readonly MainWindow gui;
        private readonly WorktimesRepository repository = WorktimesRepository.Instance;
        private string currentlySelectedMonth = "";

        private string currentlySelectedYear = "";

        public WorktimesController(MainWindow gui)
        {
            DateSystem.LicenseKey = "LostTimeIsNeverFoundAgain";
            this.gui = gui;
            this.gui.YearSelection.SelectionChanged += YearSelectionOnSelectionChanged;
            this.gui.MonthSelection.SelectionChanged += MonthSelectionOnSelectionChanged;
            this.gui.Refresh.Click += (sender, args) => Refresh();
            this.gui.SettingsSaved += (sender, args) => Refresh();
            FillYearAndMonthSelections();
            CreateHeader();
            Refresh();

            if (!userSettings.Settings.AutoRefreshEnabled)
            {
                return;
            }

            DispatcherTimer autoRefresh = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(userSettings.Settings.AutoRefreshEveryXMinutes)
            };
            autoRefresh.Tick += AutoRefresh;
            autoRefresh.Start();
        }

        private void AutoRefresh(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            ClearData();
            FillData();
            gui.RepaintTable(null, null);
            gui.LastRefresh.Content = DateTime.Now.TimeOfDay.ToString("c").Substring(0, 8);
        }

        private void FillYearAndMonthSelections()
        {
            if ("0".Equals(userSettings.Settings.StartingYear) || string.IsNullOrEmpty(userSettings.Settings.StartingYear))
            {
                Worktimes w = repository.FindById(1) ?? repository.FindToday();
                userSettings.Settings.StartingYear = w.Day.Substring(0, 4);
            }

            int year = int.Parse(userSettings.Settings.StartingYear);

            for (int y = year; y <= DateTime.Now.Year; y++)
            {
                Label lblYear = new Label { Content = y };
                gui.YearSelection.Items.Add(lblYear);
                if (y.Equals(DateTime.Now.Year))
                {
                    gui.YearSelection.SelectedItem = lblYear;
                }
            }

            for (int m = 1; m <= 12; m++)
            {
                Label lblMonth = new Label
                {
                    Content = m.ToMonthName(),
                    Tag = m
                };
                gui.MonthSelection.Items.Add(lblMonth);
                if (m.Equals(DateTime.Now.Month))
                {
                    gui.MonthSelection.SelectedItem = lblMonth;
                }
            }
        }

        private void MonthSelectionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedMonth =
                ((Label)((ComboBox)sender).SelectedItem).Tag.ToString();
            Refresh();
        }

        private void YearSelectionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedYear =
                ((Label)((ComboBox)sender).SelectedItem).Content.ToString();
            Refresh();
        }

        private void CreateHeader()
        {
            gui.AddHeader(new[]
            {
                "Day", "Starting Time", "Worktime", "Break Sum", "Ending Time", "Daily OT Minutes", "Sick Leave",
                "Vacation", "Public Holiday"
            });
        }

        private void FillData()
        {
            List<Worktimes> wts = repository.FindAllByYearAndMonth(currentlySelectedYear, currentlySelectedMonth);
            if (wts == null)
            {
                return;
            }

            string dayStartString = $"{currentlySelectedYear}-{currentlySelectedMonth.PadLeft(2, '0')}";
            double sumWorktime = 0;
            double sumWorktimeWeekly = 0;
            int sumPause = 0;
            int sumPauseWeekly = 0;
            double dailyOt = 0;
            double weeklyOt = 0;
            int daysThisWeek = 0;
            for (int i = 1; i <= 31; i++)
            {
                try
                {
                    if (SkipWeekends(i))
                    {
                        continue;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }

                string day = dayStartString + $"-{i.ToString().PadLeft(2, '0')}";
                if (wts.Count(w => w.Day.Equals(day)) != 0)
                {
                    Worktimes wt = wts.First(w => w.Day.Equals(day));
                    sumWorktime += AddDataRow(wt, ref sumWorktimeWeekly, ref sumPause, ref sumPauseWeekly, ref dailyOt,
                        ref weeklyOt);
                    if (!wt.IsVacation && !wt.IsSickLeave)
                    {
                        daysThisWeek++;
                    }
                }
                else
                {
                    AddEmptyRow(day);
                    if (!DateSystem.IsPublicHoliday(day.ToDateTime(), CountryCode.DE))
                    {
                        daysThisWeek++;
                    }
                }

                if (userSettings.Settings.WeeklySummaries &&
                    ((!userSettings.Settings.ShowWeekends &&
                      (new DateTime(int.Parse(currentlySelectedYear), int.Parse(currentlySelectedMonth), i).DayOfWeek ==
                       DayOfWeek.Friday))
                     ||
                     (userSettings.Settings.ShowWeekends &&
                      (new DateTime(int.Parse(currentlySelectedYear), int.Parse(currentlySelectedMonth), i).DayOfWeek ==
                       DayOfWeek.Sunday))
                     ||
                     IsLastDayOfMonth(i)))
                {
                    gui.AddRow(new[]
                    {
                        "Weekly:",
                        "",
                        sumWorktimeWeekly.ToString(Format, CultureInfo.CurrentCulture) + " / "
                        + userSettings.Settings.WorkhoursPerWeek / userSettings.Settings.WorkdaysPerWeek * daysThisWeek,
                        utilities.Time.SecondsToMinutes(sumPauseWeekly).ToString(Format, CultureInfo.CurrentCulture),
                        "",
                        weeklyOt.ToString(Format, CultureInfo.CurrentCulture)
                    }, true);
                    sumWorktimeWeekly = 0;
                    sumPauseWeekly = 0;
                    weeklyOt = 0;
                    daysThisWeek = 0;
                }

                if (IsLastDayOfMonth(i))
                {
                    break;
                }
            }

            gui.AddSumRow(new[]
            {
                "All: ", "", sumWorktime.ToString(Format, CultureInfo.CurrentCulture),
                utilities.Time.SecondsToHours(sumPause).ToString(Format, CultureInfo.CurrentCulture),
                "",
                dailyOt.ToString(Format, CultureInfo.CurrentCulture)
            });
        }

        private double AddDataRow(Worktimes wt,
            ref double sumWorktimeWeekly, ref int sumPause, ref int sumPauseWeekly, ref double dailyOt,
            ref double weeklyOt)
        {
            double differenceToday = 0;
            bool isPublicHoliday = DateSystem.IsPublicHoliday(wt.Day.ToDateTime(), CountryCode.DE);
            double sumWorktime = 0;
            if (!wt.IsVacation && !wt.IsSickLeave && !isPublicHoliday)
            {
                differenceToday =
                    utilities.Time.HoursToMinutes(
                        (float)(wt.Worktime - userSettings.Settings.CalculateBreakTime(wt) - userSettings.Settings.WorkhoursPerDay));
                sumWorktime = wt.Worktime - utilities.Time.SecondsToHours(wt.Pause);
                sumWorktimeWeekly += wt.Worktime - utilities.Time.SecondsToHours(wt.Pause);
                sumPause += wt.Pause;
                sumPauseWeekly += wt.Pause;
            }

            if (!wt.Day.Equals(DateTime.Today.ToCustomString()) ||
                !userSettings.Settings.CurrentDayExcludedFromOvertimeCalculation)
            {
                dailyOt += differenceToday;
                weeklyOt += differenceToday;
            }

            List<UIElement> elements = gui.AddRow(new[]
            {
                wt.Day,
                wt.StartingTime.ToString(),
                wt.Worktime.ToString(Format, CultureInfo.CurrentCulture) + " / " +
                userSettings.Settings.WorkhoursPerDay.ToString(Format, CultureInfo.CurrentCulture),
                utilities.Time.SecondsToMinutes(wt.Pause).ToString(Format, CultureInfo.CurrentCulture),
                wt.StartingTime.AddSeconds((int)utilities.Time.HoursToSeconds((float)wt.Worktime)).ToString(),
                differenceToday.ToString(Format, CultureInfo.CurrentCulture),
                wt.IsSickLeave.ToString(),
                wt.IsVacation.ToString(),
                isPublicHoliday.ToString()
            }, userSettings.Settings.CurrentDayBold && IsDayToday(wt.Day));
            foreach (UIElement uiElement in elements)
            {
                if (uiElement is CheckBox box)
                {
                    box.Click += CheckBoxOnClick;
                }
            }

            return sumWorktime;
        }

        private static bool IsDayToday(string day)
        {
            return day.ToDateTime().Equals(DateTime.Today);
        }

        private bool SkipWeekends(int i)
        {
            if (userSettings.Settings.ShowWeekends)
            {
                return false;
            }

            DateTime d = new DateTime(int.Parse(currentlySelectedYear), int.Parse(currentlySelectedMonth), i);
            return (d.DayOfWeek == DayOfWeek.Saturday) || (d.DayOfWeek == DayOfWeek.Sunday);
        }

        private bool IsLastDayOfMonth(int dayOfMonth)
        {
            switch (dayOfMonth)
            {
                case 28 when !userSettings.Settings.IsLeapYear(int.Parse(currentlySelectedYear)) &&
                             "2".Equals(currentlySelectedMonth):

                case 29
                    when userSettings.Settings.IsLeapYear(int.Parse(currentlySelectedYear)) && "2".Equals(currentlySelectedMonth):

                case 30 when "4".Equals(currentlySelectedMonth) || "6".Equals(currentlySelectedMonth) ||
                             "9".Equals(currentlySelectedMonth) || "11".Equals(currentlySelectedMonth):

                case 31 when "1".Equals(currentlySelectedMonth) || "3".Equals(currentlySelectedMonth) ||
                             "5".Equals(currentlySelectedMonth) || "7".Equals(currentlySelectedMonth) ||
                             "8".Equals(currentlySelectedMonth) || "10".Equals(currentlySelectedMonth) ||
                             "12".Equals(currentlySelectedMonth):
                    return true;
                default:
                    return false;
            }
        }

        private void CheckBoxOnClick(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            switch (int.Parse(box.Tag.ToString().Substring(0, 1)))
            {
                case IndexSickLeave:
                    {
                        Worktimes wt = repository.FindByDay(box.Tag.ToString().Substring(2));
                        wt.IsSickLeave = box.IsChecked == true;
                        repository.Save(wt);
                        break;
                    }
                case IndexVacation:
                    {
                        Worktimes wt = repository.FindByDay(box.Tag.ToString().Substring(2));
                        wt.IsVacation = box.IsChecked == true;
                        repository.Save(wt);
                        break;
                    }
            }

            Refresh();
        }

        private void AddEmptyRow(string day)
        {
            List<UIElement> elements = gui.AddRow(new[]
            {
                day, 0.ToString(), 0.ToString(), 0.ToString(), 0.ToString(), 0.ToString(), false.ToString(),
                false.ToString(), DateSystem.IsPublicHoliday(day.ToDateTime(), CountryCode.DE).ToString()
            }, userSettings.Settings.CurrentDayBold && IsDayToday(day));
            foreach (UIElement uiElement in elements)
            {
                if (uiElement is CheckBox box)
                {
                    box.Click += CheckBoxOnClick;
                }
            }
        }

        private void ClearData()
        {
            gui.DataGrid.Children.Clear();
            gui.DataGrid.RowDefinitions.Clear();
            gui.ClearSumRow();
        }
    }
}