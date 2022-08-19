using System.Windows;

namespace WorktimeSummary
{
    using System.Windows.Controls;
    using userSettings;

    public partial class UserSettings : Window
    {
        public UserSettings()
        {
            InitializeComponent();
        }

        private void ThemeSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Settings.TableThemeTitle = ((Label)((DockPanel)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content).Children[1]).Content.ToString();
            Settings.TableTheme1 = ((Border)((DockPanel)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content).Children[1]).Background;
            Settings.TableTheme2 = ((Border)((DockPanel)((ComboBoxItem)((ComboBox)sender).SelectedItem).Content).Children[1]).Background;
        }
    }
}