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
        private readonly IssuesRepository issuesRepository = IssuesRepository.Instance;
        private readonly WorktimesRepository repository = WorktimesRepository.Instance;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly TimerWindow timerWindow;
        private Issues issue;
        private bool issueTracking;
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
            this.timerWindow.ToggleIssueTracking.Click += ToggleIssueTrackingOnClick;
            this.timerWindow.AddOneFunctionPoint.Click += AddOneFunctionPointOnClick;
        }

        private bool IsBreak { get; set; }

        private bool Break30Minutes { get; set; }

        private bool Break45Minutes { get; set; }

        private void AddOneFunctionPointOnClick(object sender, RoutedEventArgs e)
        {
            if (issue == null)
            {
                return;
            }

            AddOneFunctionPoint();
        }

        private void AddOneFunctionPoint()
        {
            if (string.IsNullOrEmpty(timerWindow.IssueNumber.Text) || (issue == null))
            {
                return;
            }

            issue.FunctionPoints++;
            timerWindow.IssueFunctionPoints.Content = issue.FunctionPoints;
        }

        private void ToggleIssueTrackingOnClick(object sender, RoutedEventArgs e)
        {
            ToggleIssueTracking();
        }

        private void ToggleIssueTracking()
        {
            if (string.IsNullOrEmpty(timerWindow.IssueNumber.Text))
            {
                return;
            }

            if ((issue == null) || !issue.IssueNumber.Equals(timerWindow.IssueNumber.Text))
            {
                issue = new Issues();
                timerWindow.IssueTrackingMinutes.Content = "0.00";
                timerWindow.IssueFunctionPoints.Content = "0";
            }

            if (issueTracking)
            {
                issuesRepository.Save(issue);
            }

            issue.IssueNumber = timerWindow.IssueNumber.Text;
            if (issuesRepository.FindByIssueNumber(issue.IssueNumber) != null)
            {
                issue = issuesRepository.FindByIssueNumber(issue.IssueNumber);
                timerWindow.IssueFunctionPoints.Content = issue.FunctionPoints;
            }

            issueTracking = !issueTracking;
        }

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
                if (issueTracking)
                {
                    issue.Seconds++;
                    timerWindow.IssueTrackingMinutes.Content = Time.SecondsToMinutes(issue.Seconds).ToString("0.00");
                    if ((issue.Seconds % Time.MinutesToSeconds(30)) == 0)
                    {
                        AddOneFunctionPoint();
                    }
                }

                if (!Break45Minutes && ((worktimes.WorktimeInSeconds - worktimes.Pause) > Time.HoursToSeconds(9)))
                {
                    worktimes.Pause = Math.Max(Time.MinutesToSeconds(45), worktimes.Pause);
                    Break45Minutes = true;
                }
                else if (!Break30Minutes && ((worktimes.WorktimeInSeconds - worktimes.Pause) > Time.HoursToSeconds(6)))
                {
                    worktimes.Pause = Math.Max(Time.MinutesToSeconds(30), worktimes.Pause);
                    Break30Minutes = true;
                }
            }

            worktimes.WorktimeInSeconds = (long)(Time.Now() - worktimes.StartingTime).ToSeconds();
            timerWindow.WorktimeDecimal.Content = worktimes.Worktime.ToString(DefaultStringForTimeDecimals);
            timerWindow.WorktimeTime.Content = HourDecimalToTimeString(worktimes.Worktime);
            timerWindow.BreakDecimal.Content =
                Time.SecondsToHours(worktimes.Pause).ToString(DefaultStringForTimeDecimals);
            timerWindow.BreakTime.Content = HourDecimalToTimeString(Time.SecondsToHours(worktimes.Pause));

            SetEstimatedEndingTime();

            if ((Settings.AutoSaveEveryXMinutes <= 0) ||
                ((worktimes.WorktimeInSeconds % Time.MinutesToSeconds(Settings.AutoSaveEveryXMinutes)) != 0))
            {
                return;
            }

            repository.Save(worktimes);
        }

        private static string HourDecimalToTimeString(double worktimesWorktime)
        {
            int seconds = Time.HoursToSeconds((float)worktimesWorktime);
            float minutes = Time.SecondsToMinutes(seconds);
            seconds -= Time.MinutesToSeconds(minutes);
            float hours = Time.MinutesToHours(minutes);
            minutes -= Time.HoursToMinutes(hours);

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

            if ((worktimes.StartingTime == null) || worktimes.StartingTime.ToString().Equals(DefaultStringForTimes))
            {
                worktimes.StartingTime = Time.Now();
                worktimes.StartingTime = worktimes.StartingTime.AddMilliseconds(-worktimes.StartingTime.Milliseconds);
            }
            else if (!timer.IsEnabled)
            {
                MessageBoxResult r = MessageBox.Show(
                    "There is already a worktime for today. Do you want to add a break for the meantime?",
                    "Workday already exists", MessageBoxButton.YesNoCancel, MessageBoxImage.Question,
                    MessageBoxResult.No);
                if (r.Equals(MessageBoxResult.Yes))
                {
                    Time diff = Time.Now() -
                                worktimes.StartingTime.AddSeconds(Time.HoursToSeconds((float)worktimes.Worktime));
                    worktimes.Pause += (int)diff.ToSeconds();
                }
            }

            SetEstimatedEndingTime();

            timerWindow.StartingTime.Content = worktimes.StartingTime;
            timer.Start();
        }

        private void SetEstimatedEndingTime()
        {
            Time end = CalculateEstimatedEndingTime(worktimes.StartingTime);
            if (end.ToString().Length > 5)
            {
                timerWindow.EstEndingTime.Content = end.ToString();
            }
        }

        private Time CalculateEstimatedEndingTime(Time worktimesStartingTime)
        {
            float dailyHoursToWork = Settings.WorkhoursPerDay;

            Time end = worktimesStartingTime.AddHours(dailyHoursToWork);
            if ((worktimes.Worktime > 9) && (worktimes.Pause > Time.MinutesToSeconds(45)))
            {
                end = end.AddSeconds(worktimes.Pause - Time.MinutesToSeconds(45));
            }
            else if ((worktimes.Worktime > 6) && (worktimes.Pause > Time.MinutesToSeconds(30)))
            {
                end = end.AddSeconds(worktimes.Pause - Time.MinutesToSeconds(30));
            }

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
            if (issueTracking)
            {
                ToggleIssueTracking();
            }
        }
    }
}