namespace WorktimeSummary
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using userSettings;

    public partial class UserSettingsWindow : Window
    {
        public UserSettingsWindow()
        {
            InitializeComponent();
            SelectCorrectThemeComboBoxItem();
            LoadSettings();
        }

        public event EventHandler<EventArgs> TableThemeChanged;

        private void LoadSettings()
        {
            HoursPerWeek.Text = Settings.WorkhoursPerWeek.ToString(CultureInfo.CurrentCulture);
            ShowWeekends.IsChecked = Settings.ShowWeekends;
            EnableAutoRefresh.IsChecked = Settings.AutoRefreshEnabled;
            AutoRefreshPanel.IsEnabled = EnableAutoRefresh.IsChecked == true;
            for (int i = 0; i < AutoRefresh.Items.Count; i++)
            {
                if (!int.Parse(((Label)AutoRefresh.Items[i]).Content.ToString())
                        .Equals(Settings.AutoRefreshEveryXMinutes))
                {
                    continue;
                }

                AutoRefresh.SelectedIndex = i;
                break;
            }
            for (int i = 0; i < AutoSave.Items.Count; i++)
            {
                if (!int.Parse(((Label)AutoSave.Items[i]).Content.ToString())
                        .Equals(Settings.AutoSaveEveryXMinutes))
                {
                    continue;
                }

                AutoSave.SelectedIndex = i;
                break;
            }
        }

        private void SelectCorrectThemeComboBoxItem()
        {
            if (string.IsNullOrEmpty(Settings.TableThemeTitle))
            {
                ThemeSelection.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < ThemeSelection.Items.Count; i++)
                {
                    if (((Label)((DockPanel)((ComboBoxItem)ThemeSelection.Items[i]).Content).Children[0]).Content
                        .ToString() != Settings.TableThemeTitle)
                    {
                        continue;
                    }

                    ThemeSelection.SelectedIndex = i;
                    break;
                }
            }
        }

        private void ThemeSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.TableThemeTitle =
                ((Label)((DockPanel)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content).Children[0]).Content
                .ToString();
            Settings.TableTheme1 =
                ((Border)((DockPanel)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content).Children[1]).Background;
            Settings.TableTheme2 =
                ((Border)((DockPanel)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content).Children[3]).Background;

            TableThemeChanged?.Invoke(ThemeSelection, EventArgs.Empty);
        }

        private void EnableAutoRefresh_OnChecked(object sender, RoutedEventArgs e)
        {
            AutoRefreshPanel.IsEnabled = true;
        }

        private void EnableAutoRefresh_OnUnchecked(object sender, RoutedEventArgs e)
        {
            AutoRefreshPanel.IsEnabled = false;
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.WorkhoursPerWeek = float.Parse(HoursPerWeek.Text);
            Settings.ShowWeekends = ShowWeekends.IsChecked == true;
            Settings.AutoRefreshEnabled = EnableAutoRefresh.IsChecked == true;
            if (EnableAutoRefresh.IsChecked == true)
            {
                Settings.AutoRefreshEveryXMinutes = int.Parse(((Label)AutoRefresh.SelectedItem).Content.ToString());
            }

            Settings.CurrentDayBold = CurrentDayBold.IsChecked == true;
            Settings.AutoSaveEveryXMinutes = int.Parse(((Label)AutoSave.SelectedItem).Content.ToString());
            Close();
        }
    }
}