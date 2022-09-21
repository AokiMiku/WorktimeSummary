namespace WorktimeSummary.controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using data;
    using repositories;
    using userSettings;

    public class WorktimesController
    {
        private const int IndexSickLeave = 6;
        private const int IndexVacation = 7;
        private readonly MainWindow gui;
        private readonly WorktimesRepository repository = new WorktimesRepository();
        private string currentlySelectedMonth = "";

        private string currentlySelectedYear = "";

        public WorktimesController(MainWindow gui)
        {
            this.gui = gui;
            this.gui.YearSelection.SelectionChanged += YearSelectionOnSelectionChanged;
            this.gui.MonthSelection.SelectionChanged += MonthSelectionOnSelectionChanged;
            this.gui.Refresh.Click += (sender, args) => Refresh();
            FillYearAndMonthSelections();
            CreateHeader();
            Refresh();
            gui.LastRefresh.Content = DateTime.Now.TimeOfDay.ToString("c").Substring(0, 8);
            if (Settings.AutoRefreshEnabled)
            {
                DispatcherTimer autoRefresh = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMinutes(Settings.AutoRefreshEveryXMinutes)
                };
                autoRefresh.Tick += AutoRefresh;
                autoRefresh.Start();
            }
        }

        private void AutoRefresh(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            ClearData();
            FillData();
            gui.RepaintTable();
            gui.LastRefresh.Content = DateTime.Now.TimeOfDay.ToString("c").Substring(0, 8);
        }

        private void FillYearAndMonthSelections()
        {
            if ("0".Equals(Settings.StartingYear))
            {
                Worktimes w = repository.FindById(1);
                Settings.StartingYear = w.Day.Substring(0, 4);
            }

            int year = int.Parse(Settings.StartingYear);

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
                    Content = new DateTime(DateTime.Now.Year, m, 1)
                        .ToString("MMMM", CultureInfo.InvariantCulture),
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
                "Vacation"
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
            int sumPause = 0;
            float dailyHoursToWork = Settings.WorkhoursPerWeek / 5;
            double differencesInDailyHours = 0;
            const string format = "0.##";
            for (int i = 1; i <= 31; i++)
            {
                if ("2".Equals(currentlySelectedMonth) &&
                    ((Settings.IsLeapYear(int.Parse(currentlySelectedYear)) && i == 30) ||
                     (!Settings.IsLeapYear(int.Parse(currentlySelectedYear)) && i == 29)))
                {
                    break;
                }

                if (i == 31 && ("4".Equals(currentlySelectedMonth) || "6".Equals(currentlySelectedMonth) ||
                                "9".Equals(currentlySelectedMonth) || "11".Equals(currentlySelectedMonth)))
                {
                    break;
                }

                if (!Settings.ShowWeekends)
                {
                    DateTime d = new DateTime(int.Parse(currentlySelectedYear), int.Parse(currentlySelectedMonth), i);
                    if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }
                }

                string day = dayStartString + $"-{i.ToString().PadLeft(2, '0')}";
                if (wts.Count(w => w.Day.Equals(day)) != 0)
                {
                    Worktimes wt = wts.First(w => w.Day.Equals(day));
                    double differenceToday = 0;
                    if (!wt.IsVacation && !wt.IsSickLeave)
                    {
                        differenceToday = (wt.Worktime - wt.Pause / 3600d - dailyHoursToWork) * 60d;
                    }

                    differencesInDailyHours += differenceToday;

                    List<UIElement> elements = gui.AddRow(new[]
                    {
                        wt.Day,
                        wt.StartingTime.ToString(),
                        wt.Worktime.ToString(format, CultureInfo.CurrentCulture),
                        (wt.Pause / 60f).ToString(format, CultureInfo.CurrentCulture),
                        wt.StartingTime.AddSeconds((int)(wt.Worktime * 3600)).ToString(),
                        differenceToday.ToString(format, CultureInfo.CurrentCulture),
                        wt.IsSickLeave.ToString(),
                        wt.IsVacation.ToString()
                    });
                    foreach (UIElement uiElement in elements)
                    {
                        if (uiElement is CheckBox box)
                        {
                            box.Click += CheckBoxOnClick;
                        }
                    }

                    if (!wt.IsVacation && !wt.IsSickLeave)
                    {
                        sumWorktime += wt.Worktime;
                        sumPause += wt.Pause;
                    }
                }
                else
                {
                    AddEmptyRow(day);
                }
            }

            gui.AddSumRow(new[]
            {
                "All: ", "", sumWorktime.ToString(format, CultureInfo.CurrentCulture),
                ((double)sumPause / 3600).ToString(format, CultureInfo.CurrentCulture),
                "",
                differencesInDailyHours.ToString(format, CultureInfo.CurrentCulture)
            });
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
                false.ToString()
            });
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