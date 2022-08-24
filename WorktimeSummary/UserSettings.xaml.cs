using System.Windows;

namespace WorktimeSummary
{
    using System;
    using System.Globalization;
    using System.Windows.Controls;
    using userSettings;

    public partial class UserSettings : Window
    {
        public event EventHandler<EventArgs> TableThemeChanged;

        public UserSettings()
        {
            InitializeComponent();
            SelectCorrectThemeComboBoxItem();
            LoadSettings();
        }

        private void LoadSettings()
        {
            HoursPerWeek.Text = Settings.WorkhoursPerWeek.ToString(CultureInfo.CurrentCulture);
            ShowWeekends.IsChecked = Settings.ShowWeekends;
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

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.WorkhoursPerWeek = float.Parse(HoursPerWeek.Text);
            Settings.ShowWeekends = ShowWeekends.IsChecked == true;
            Close();
        }
    }
}