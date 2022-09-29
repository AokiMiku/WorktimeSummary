namespace WorktimeSummary.controllers
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;
    using data;
    using repositories;
    using userSettings;
    using utilities;

    public class TimerController
    {
        private const string DefaultStringForTimes = "00:00:00";
        private const string DefaultStringForTimeDecimals = "0.00000";
        private readonly WorktimesRepository repository = WorktimesRepository.Instance;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly TimerWindow timerWindow;
        private Worktimes worktimes;

        public TimerController(TimerWindow timerWindow)
        {
            this.timerWindow = timerWindow;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerOnTick;
            this.timerWindow.Start.Click += StartOnClick;
            this.timerWindow.Break.Click += BreakOnClick;
            this.timerWindow.Stop.Click += StopOnClick;
            this.timerWindow.Closing += TimerWindowOnClosing;
        }

        private bool IsBreak { get; set; }

        private bool Break30Minutes { get; set; }

        private bool Break45Minutes { get; set; }

        private void TimerWindowOnClosing(object sender, CancelEventArgs e)
        {
            if (timer.IsEnabled)
            {
                Stop();
            }
        }

        private void StartOnClick(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void BreakOnClick(object sender, RoutedEventArgs e)
        {
            BreakToggle();
        }

        private void StopOnClick(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            if (IsBreak)
            {
                worktimes.Pause++;
            }
            else
            {
                if (!Break45Minutes && worktimes.WorktimeInSeconds - worktimes.Pause > 9 * 3600)
                {
                    worktimes.Pause = Math.Max(45 * 60, worktimes.Pause);
                    Break45Minutes = true;
                }
                else if (!Break30Minutes && worktimes.WorktimeInSeconds - worktimes.Pause > 6 * 3600)
                {
                    worktimes.Pause = Math.Max(30 * 60, worktimes.Pause);
                    Break30Minutes = true;
                }
            }

            worktimes.WorktimeInSeconds = (long)(Time.Now() - worktimes.StartingTime).ToSeconds();
            timerWindow.WorktimeDecimal.Content = worktimes.Worktime.ToString("0.00000");
            timerWindow.WorktimeTime.Content = HourDecimalToTimeString(worktimes.Worktime);
            timerWindow.BreakDecimal.Content = (worktimes.Pause / 3600d).ToString("0.00000");
            timerWindow.BreakTime.Content = HourDecimalToTimeString(worktimes.Pause / 3600d);

            if (Settings.AutoSaveEveryXMinutes <= 0 ||
                worktimes.WorktimeInSeconds % (Settings.AutoSaveEveryXMinutes * 60) != 0)
            {
                return;
            }

            repository.Save(worktimes);
        }

        private string HourDecimalToTimeString(double worktimesWorktime)
        {
            long seconds = (long)(worktimesWorktime * 3600d);
            long minutes = seconds / 60;
            seconds -= minutes * 60;
            long hours = minutes / 60;
            minutes -= hours * 60;

            return $"{hours:00}:{minutes:00}:{seconds:00}";
        }

        private void Start()
        {
            if (timer.IsEnabled)
            {
                Stop();
                timerWindow.WorktimeTime.Content = DefaultStringForTimes;
                timerWindow.WorktimeDecimal.Content = DefaultStringForTimeDecimals;
            }

            worktimes = repository.FindToday();

            if (worktimes.StartingTime == null || worktimes.StartingTime.ToString().Equals(DefaultStringForTimes))
            {
                worktimes.StartingTime = Time.Now();
                worktimes.StartingTime = worktimes.StartingTime.AddMilliseconds(-worktimes.StartingTime.Milliseconds);
            }

            Time end = CalculateEstimatedEndingTime(worktimes.StartingTime);
            if (end.ToString().Length > 5)
            {
                timerWindow.EstEndingTime.Content = end.ToString();
            }

            timerWindow.StartingTime.Content = worktimes.StartingTime;
            timer.Start();
        }

        private Time CalculateEstimatedEndingTime(Time worktimesStartingTime)
        {
            Time end = worktimesStartingTime.AddHours(8);
            end = end.AddMinutes(30);
            return end;
        }

        private void BreakToggle()
        {
            IsBreak = !IsBreak;
        }

        private void Stop()
        {
            timer.Stop();
            repository.Save(worktimes);
        }
    }
}