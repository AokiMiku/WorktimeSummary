namespace WorktimeSummary
{
    using System.Windows;
    using controllers;

    public partial class TimerWindow : Window
    {
        public TimerWindow()
        {
            InitializeComponent();
            TimerController dummy = new TimerController(this);
        }
    }
}