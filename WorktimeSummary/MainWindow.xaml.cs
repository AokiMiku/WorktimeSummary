namespace WorktimeSummary
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using controllers;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WorktimesController dummy = new WorktimesController(this);
        }

        public void AddRow(string[] values, bool isHeader = false)
        {
            DataGrid.RowDefinitions.Add(new RowDefinition());
            DataGrid.RowDefinitions[DataGrid.RowDefinitions.Count - 1].MinHeight = isHeader ? 40 : 25;
            for (int i = 0; i < 4; i++)
            {
                if (!isHeader)
                {
                    Border background = new Border
                    {
                        Background = DataGrid.RowDefinitions.Count % 2 == 0 ? Brushes.LightGray : Brushes.DarkGray
                    };
                    Grid.SetColumn(background, i);
                    Grid.SetRow(background, DataGrid.RowDefinitions.Count - 1);
                    Grid.SetRowSpan(background, DataGrid.ColumnDefinitions.Count);
                    DataGrid.Children.Add(background);
                }

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
    }
}