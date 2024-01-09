namespace WorktimeSummary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using controllers;
    using userSettings;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static Brush defaultRowBackgroundZebra1;
        private static Brush defaultRowBackgroundZebra2;
        private static readonly Brush DefaultRowBackgroundHover = Brushes.SlateGray;
        private readonly List<Label> sumLabels = new List<Label>();

        private Brush currentlyHoveredRowBackground;

        public MainWindow()
        {
            InitializeComponent();
            defaultRowBackgroundZebra1 = Settings.TableTheme1;
            defaultRowBackgroundZebra2 = Settings.TableTheme2;
            WorktimesController dummy = new WorktimesController(this);
        }

        public event EventHandler<EventArgs> SettingsSaved;

        public void AddHeader(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Label lbl = new Label
                {
                    Content = values[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(lbl, i);
                // Grid.SetColumnSpan(lbl, 2);
                Grid.SetRow(lbl, 0);
                ((Grid)HeaderRow.Parent).Children.Add(lbl);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="bold">If the texts of the current row should be bold.</param>
        /// <param name="values"></param>
        /// <returns>List of created UIElements</returns>
        public List<UIElement> AddRow(string[] values, bool bold = false)
        {
            List<UIElement> elements = new List<UIElement>();
            RowDefinition row;
            DataGrid.RowDefinitions.Add(row = new RowDefinition());
            row.MinHeight = 25;
            row.MaxHeight = 40;

            Border background = new Border();
            background.MouseEnter += BackgroundOnMouseEnter;
            background.MouseLeave += BackgroundOnMouseLeave;
            Grid.SetColumn(background, 0);
            Grid.SetRow(background, DataGrid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(background, DataGrid.ColumnDefinitions.Count);
            DataGrid.Children.Add(background);

            for (int i = 0; i < values.Length; i++)
            {
                UIElement addToRow;
                if (values[i].ToLower().Equals("false") || values[i].ToLower().Equals("true"))
                {
                    CheckBox chk = new CheckBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = i + ":" + values[0],
                        IsChecked = bool.Parse(values[i])
                    };
                    addToRow = chk;
                }
                else
                {
                    Label lbl = new Label
                    {
                        Content = values[i],
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = background
                    };
                    if (bold)
                    {
                        lbl.FontWeight = FontWeights.Bold;
                    }

                    addToRow = lbl;
                }

                Grid.SetColumn(addToRow, i);
                Grid.SetRow(addToRow, DataGrid.RowDefinitions.Count - 1);
                DataGrid.Children.Add(addToRow);
                addToRow.MouseEnter += LblOnMouseEnter;
                addToRow.MouseLeave += LblOnMouseLeave;
                elements.Add(addToRow);
            }

            return elements;
        }

        public void AddSumRow(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Label lbl = new Label
                {
                    Content = values[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(lbl, i);
                // Grid.SetColumnSpan(lbl, 2);
                Grid.SetRow(lbl, 2);
                ((Grid)SumRow.Parent).Children.Add(lbl);
                sumLabels.Add(lbl);
            }
        }

        public void ClearSumRow()
        {
            foreach (Label lbl in sumLabels)
            {
                ((Grid)SumRow.Parent).Children.Remove(lbl);
            }

            sumLabels.Clear();
        }

        private void LblOnMouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (!(element.Tag is Border))
            {
                return;
            }

            BackgroundOnMouseLeave(element.Tag, e);
        }

        private void LblOnMouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (!(element.Tag is Border))
            {
                return;
            }

            BackgroundOnMouseEnter(element.Tag, e);
        }

        private void BackgroundOnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            ((Border)sender).Background = currentlyHoveredRowBackground;
        }

        private void BackgroundOnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            currentlyHoveredRowBackground = ((Border)sender).Background;
            ((Border)sender).Background = DefaultRowBackgroundHover;
        }

        private void UserSettings_OnClick(object sender, RoutedEventArgs e)
        {
            UserSettingsWindow us = new UserSettingsWindow();
            us.TableThemeChanged += RepaintTable;
            us.SettingsSaved += UsOnSettingsSaved;
            us.ShowDialog();
        }

        private void UsOnSettingsSaved(object sender, EventArgs e)
        {
            SettingsSaved?.Invoke(sender, e);
        }

        public void RepaintTable(object sender, EventArgs e)
        {
            defaultRowBackgroundZebra1 = Settings.TableTheme1;
            defaultRowBackgroundZebra2 = Settings.TableTheme2;

            for (int i = 0; i < DataGrid.Children.Count; i++)
            {
                if (!(DataGrid.Children[i] is Border))
                {
                    continue;
                }

                ((Border)DataGrid.Children[i]).Background = (Grid.GetRow(DataGrid.Children[i]) % 2) == 0
                    ? defaultRowBackgroundZebra1
                    : defaultRowBackgroundZebra2;
            }
        }

        private void ButtonMonthLeft_OnClick(object sender, RoutedEventArgs e)
        {
            if (MonthSelection.SelectedIndex > 0)
            {
                MonthSelection.SelectedIndex--;
            }
            else
            {
                SelectNextYear(false);
            }
        }

        private void ButtonMonthRight_OnClick(object sender, RoutedEventArgs e)
        {
            if (MonthSelection.SelectedIndex < (MonthSelection.Items.Count - 1))
            {
                MonthSelection.SelectedIndex++;
            }
            else
            {
                SelectNextYear(true);
            }
        }

        private void SelectNextYear(bool changeYearSelectionForward)
        {
            if (changeYearSelectionForward && (YearSelection.SelectedIndex < (YearSelection.Items.Count - 1)))
            {
                YearSelection.SelectedIndex++;
                MonthSelection.SelectedIndex = 0;
            }
            else if (!changeYearSelectionForward && (YearSelection.SelectedIndex > 0))
            {
                YearSelection.SelectedIndex--;
                MonthSelection.SelectedIndex = MonthSelection.Items.Count - 1;
            }
        }

        private void LaunchTimer_OnClick(object sender, RoutedEventArgs e)
        {
            new TimerWindow().Show();
        }

        private void OpenOverview_OnClick(object sender, RoutedEventArgs e)
        {
            new OverviewWindow().Show();
        }
    }
}