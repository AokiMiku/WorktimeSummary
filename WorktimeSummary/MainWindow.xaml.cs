namespace WorktimeSummary
{
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
        private static readonly Brush DefaultRowBackgroundZebra1 = Brushes.DarkGray;
        private static readonly Brush DefaultRowBackgroundZebra2 = Brushes.DimGray;
        private static readonly Brush DefaultRowBackgroundHover = Brushes.SlateGray;

        private Brush currentlyHoveredRowBackground;

        public MainWindow()
        {
            InitializeComponent();
            WorktimesController dummy = new WorktimesController(this);
        }

        public void AddRow(string[] values, bool isHeader = false)
        {
            DataGrid.RowDefinitions.Add(new RowDefinition());
            DataGrid.RowDefinitions[DataGrid.RowDefinitions.Count - 1].MinHeight = isHeader ? 40 : 25;

            if (!isHeader)
            {
                Border background = new Border
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

            for (int i = 0; i < 4; i++)
            {
                Label lbl = new Label
                {
                    Content = values[i]
                };
                if (isHeader)
                {
                    lbl.FontWeight = FontWeights.Bold;
                }

                lbl.HorizontalAlignment = HorizontalAlignment.Center;
                lbl.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(lbl, i);
                Grid.SetRow(lbl, DataGrid.RowDefinitions.Count - 1);
                DataGrid.Children.Add(lbl);
            }
        }

        private void BackgroundOnMouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = currentlyHoveredRowBackground;
        }

        private void BackgroundOnMouseEnter(object sender, MouseEventArgs e)
        {
            currentlyHoveredRowBackground = ((Border)sender).Background;
            ((Border)sender).Background = DefaultRowBackgroundHover;
        }
    }
}