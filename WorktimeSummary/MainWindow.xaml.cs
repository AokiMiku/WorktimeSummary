namespace WorktimeSummary
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using controllers;
    using userSettings;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static Brush defaultRowBackgroundZebra1;
        private static Brush defaultRowBackgroundZebra2;
        private static readonly Brush DefaultRowBackgroundHover = Brushes.SlateGray;

        private Brush currentlyHoveredRowBackground;
        private readonly List<Label> sumLabels = new List<Label>();

        public MainWindow()
        {
            InitializeComponent();
            defaultRowBackgroundZebra1 = Settings.TableTheme1;
            defaultRowBackgroundZebra2 = Settings.TableTheme2;
            WorktimesController dummy = new WorktimesController(this);
        }

        public void AddHeader(string[] values)
        {
            for (int i = 0, j = 0; i < values.Length; i++)
            {
                Label lbl = new Label
                {
                    Content = values[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(lbl, i + j++ + 1);
                Grid.SetColumnSpan(lbl, 2);
                Grid.SetRow(lbl, 2);
                ((Grid)HeaderRow.Parent).Children.Add(lbl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns>List of created UIElements</returns>
        public List<UIElement> AddRow(string[] values)
        {
            List<UIElement> elements = new List<UIElement>();
            RowDefinition row;
            DataGrid.RowDefinitions.Add(row = new RowDefinition());
            row.MinHeight = 25;

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
                        Tag = i + ":" +values[0],
                        IsChecked = bool.Parse(values[i])
                    };
                    chk.Click += ChkOnClick;
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

        private void ChkOnClick(object sender, RoutedEventArgs e)
        {
            int i = int.Parse(((CheckBox)sender).Tag.ToString().Substring(0, 1));
            
        }

        public void AddSumRow(string[] values)
        {
            for (int i = 0, j = 0; i < values.Length; i++)
            {
                Label lbl = new Label
                {
                    Content = values[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetColumn(lbl, i + j++ + 1);
                Grid.SetColumnSpan(lbl, 2);
                Grid.SetRow(lbl, 4);
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UserSettings us = new UserSettings();
            us.TableThemeChanged += (o, args) => RepaintTable();
            us.ShowDialog();
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

                ((Border)DataGrid.Children[i]).Background = Grid.GetRow(DataGrid.Children[i]) % 2 == 0
                    ? defaultRowBackgroundZebra1
                    : defaultRowBackgroundZebra2;
            }
        }
    }
}