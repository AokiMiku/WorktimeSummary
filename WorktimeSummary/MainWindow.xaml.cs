namespace WorktimeSummary
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using controllers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Brush DefaultRowBackgroundZebra1 = new SolidColorBrush(Color.FromArgb(255, 237, 205, 252));
        private static readonly Brush DefaultRowBackgroundZebra2 = new SolidColorBrush(Color.FromArgb(255, 210, 182, 223));
        private static readonly Brush DefaultRowBackgroundHover = Brushes.SlateGray;

        private Brush currentlyHoveredRowBackground;

        public MainWindow()
        {
            InitializeComponent();
            WorktimesController dummy = new WorktimesController(this);
        }

        public void AddRow(string[] values, bool isHeader = false)
        {
            Border background = null;
            if (!isHeader)
            {
                RowDefinition row;
                DataGrid.RowDefinitions.Add(row = new RowDefinition());
                row.MinHeight = 25;

                background = new Border
                {
                    Background = DataGrid.RowDefinitions.Count % 2 == 0
                        ? DefaultRowBackgroundZebra1
                        : DefaultRowBackgroundZebra2
                };
                background.MouseEnter += BackgroundOnMouseEnter;
                background.MouseLeave += BackgroundOnMouseLeave;
                Grid.SetColumn(background, 0);
                Grid.SetRow(background, DataGrid.RowDefinitions.Count - 1);
                Grid.SetColumnSpan(background, DataGrid.ColumnDefinitions.Count);
                DataGrid.Children.Add(background);
            }

            int j = 0;
            for (int i = 0; i < 4; i++)
            {
                Label lbl = new Label
                {
                    Content = values[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = background
                };

                if (!isHeader)
                {
                    Grid.SetColumn(lbl, i);
                    Grid.SetRow(lbl, DataGrid.RowDefinitions.Count - 1);
                    DataGrid.Children.Add(lbl);
                    lbl.MouseEnter += LblOnMouseEnter;
                    lbl.MouseLeave += LblOnMouseLeave;
                }
                else
                {
                    lbl.FontWeight = FontWeights.Bold;
                    Grid.SetColumn(lbl, i + j++ + 1);
                    Grid.SetColumnSpan(lbl, 2);
                    Grid.SetRow(lbl, 2);
                    ((Grid)HeaderRow.Parent).Children.Add(lbl);
                }
            }
        }

        private void LblOnMouseLeave(object sender, MouseEventArgs e)
        {
            BackgroundOnMouseLeave(((Label)sender).Tag, e);
        }

        private void LblOnMouseEnter(object sender, MouseEventArgs e)
        {
            BackgroundOnMouseEnter(((Label)sender).Tag, e);
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UserSettings us = new UserSettings();
            us.ShowDialog();
        }
    }
}