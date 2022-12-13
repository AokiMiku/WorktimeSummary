using System.Windows;

namespace WorktimeSummary
{
    using System;
    using System.Net;
    using ApS.Update;
    using userSettings;

    public partial class Initilize : Window
    {
        public Initilize()
        {
            InitializeComponent();
            if (!Settings.AutoUpdate)
            {
                RedirectToMainWindow();
                return;
            }

            if (!(Settings.LastUpdate <= DateTime.Now.AddDays(-1)))
            {
                RedirectToMainWindow();
                return;
            }

            Updater updater = new Updater();
            if (updater.CheckForUpdate(Settings.LastUpdate, "AWT"))
            {
                updater.UpdateProgressChanged += UpdaterOnUpdateProgressChanged;
                updater.DownloadCompleted += (sender, args) => RedirectToMainWindow(); 
                updater.DownloadUpdateAsync();
            }
            else
            {
                RedirectToMainWindow();
            }
        }

        private void UpdaterOnUpdateProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateProgress.Maximum = e.TotalBytesToReceive;
            UpdateProgress.Value = e.BytesReceived;
            UpdateProgressText.Content = (e.BytesReceived / (double)e.TotalBytesToReceive).ToString("000.00");
        }

        private void RedirectToMainWindow()
        {
            new MainWindow().Show();
            Close();
        }
    }
}