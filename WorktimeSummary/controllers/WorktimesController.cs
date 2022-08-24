namespace WorktimeSummary.controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Windows.Controls;
    using data;
    using repositories;
    using userSettings;

    public class WorktimesController
    {
        private readonly WorktimesRepository repository = new WorktimesRepository();
        private readonly MainWindow gui;

        private string currentlySelectedYear = "";
        private string currentlySelectedMonth = "";

        public WorktimesController(MainWindow gui)
        {
            this.gui = gui;
            this.gui.YearSelection.SelectionChanged += YearSelectionOnSelectionChanged;
            this.gui.MonthSelection.SelectionChanged += MonthSelectionOnSelectionChanged;
            FillYearAndMonthSelections();
            CreateHeader();
            this.gui.RepaintTable();
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

            DateTime january = DateTime.Now.AddMonths(-DateTime.Now.Month);
            for (int m = 1; m <= 12; m++)
            {
                Label lblMonth = new Label { Content = january.AddMonths(m).Month };
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
                ((Label)((ComboBox)sender).SelectedItem).Content.ToString();
            ClearData();
            FillData();
            gui.RepaintTable();
        }

        private void YearSelectionOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedYear =
                ((Label)((ComboBox)sender).SelectedItem).Content.ToString();
            ClearData();
            FillData();
            gui.RepaintTable();
        }

        private void CreateHeader()
        {
            gui.AddHeader(new[] { "Day", "Starting Time", "Worktime", "Break Sum", "Daily Hours" });
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
            for (int i = 1; i <= 31; i++)
            {
                string day = dayStartString + $"-{i.ToString().PadLeft(2, '0')}";
                if (wts.Count(w => w.Day.Equals(day)) != 0)
                {
                    Worktimes wt = wts.First(w => w.Day.Equals(day));
                    double differenceToday = (wt.Worktime - wt.Pause / 3600d - dailyHoursToWork) * 60d;
                    differencesInDailyHours += differenceToday;
                    gui.AddRow(new[]
                    {
                        wt.Day, wt.StartingTime.ToString(),
                        wt.Worktime.ToString("0.######", CultureInfo.CurrentCulture),
                        (wt.Pause / 60).ToString(), differenceToday.ToString("0.#####", CultureInfo.CurrentCulture)
                    });
                    sumWorktime += wt.Worktime;
                    sumPause += wt.Pause;
                }
                else
                {
                    gui.AddRow(new[]
                    {
                        day, 0.ToString(), 0.ToString(), 0.ToString()
                    });
                }

                if ("2".Equals(currentlySelectedMonth) &&
                    ((Settings.IsLeapYear(int.Parse(currentlySelectedYear)) && i == 29) ||
                     (!Settings.IsLeapYear(int.Parse(currentlySelectedYear)) && i == 28)))
                {
                    break;
                }

                if (i == 30 && ("4".Equals(currentlySelectedMonth) || "6".Equals(currentlySelectedMonth) ||
                                "9".Equals(currentlySelectedMonth) || "11".Equals(currentlySelectedMonth)))
                {
                    break;
                }
            }

            gui.AddSumRow(new[]
            {
                "All: ", "", sumWorktime.ToString("0.#####", CultureInfo.CurrentCulture),
                ((double)sumPause / 3600).ToString("0.#####", CultureInfo.CurrentCulture),
                differencesInDailyHours.ToString("0.#####", CultureInfo.CurrentCulture)
            });
        }

        private void ClearData()
        {
            gui.DataGrid.Children.Clear();
            gui.DataGrid.RowDefinitions.Clear();
            gui.ClearSumRow();
        }
    }
}