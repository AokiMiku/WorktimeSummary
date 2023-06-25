using System.Windows;

namespace WorktimeSummary
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ApS.Version.MajorVersion = 1;
            ApS.Version.MinorVersion = 0;
            ApS.Version.PatchNumber = 4;

            base.OnStartup(e);
        }
    }
}