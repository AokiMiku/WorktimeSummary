using System.Windows.Controls;
using WorktimeSummary.controller;

namespace WorktimeSummary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            new WorktimesController(this);
        }

        public void AddRow(string[] values)
        {
            DataGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 4; i++)
            {
                Label lbl = new Label()
                {
                    Content = values[i]
                };
                
                Grid.SetColumn(lbl, i);
                Grid.SetRow(lbl, DataGrid.RowDefinitions.Count - 1);
                DataGrid.Children.Add(lbl);
            }
        }
    }
}