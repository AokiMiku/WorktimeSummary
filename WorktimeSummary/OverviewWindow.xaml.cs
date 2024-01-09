namespace WorktimeSummary
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using controllers;
    using userSettings;

    public partial class OverviewWindow : Window
    {
        private static Brush defaultRowBackgroundZebra1;
        private static Brush defaultRowBackgroundZebra2;
        private static readonly Brush DefaultRowBackgroundHover = Brushes.SlateGray;
        private Brush currentlyHoveredRowBackground;

        public OverviewWindow()
        {
            InitializeComponent();
            defaultRowBackgroundZebra1 = Settings.TableTheme1;
            defaultRowBackgroundZebra2 = Settings.TableTheme2;
            OverviewController dummy = new OverviewController(this);
        }

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
                Grid.SetRow(lbl, 0);
                ((Grid)HeaderRow.Parent).Children.Add(lbl);
            }
        }

        public void AddRow(string[] values, bool bold = false)
        {
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

                UIElement addToRow = lbl;

                Grid.SetColumn(addToRow, i);
                Grid.SetRow(addToRow, DataGrid.RowDefinitions.Count - 1);
                DataGrid.Children.Add(addToRow);
                addToRow.MouseEnter += LblOnMouseEnter;
                addToRow.MouseLeave += LblOnMouseLeave;
            }
        }

        public void RepaintTable()
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
    }
}