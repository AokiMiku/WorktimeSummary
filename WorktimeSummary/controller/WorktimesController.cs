using System.Globalization;

namespace WorktimeSummary.controller
{
    using WorktimeSummary.data;

    public class WorktimesController
    {
        private WorktimesRepository repository = new WorktimesRepository();
        private MainWindow gui;

        public WorktimesController(MainWindow gui)
        {
            this.gui = gui;
            this.gui.AddRow(new[]{"Day", "Starting Time", "Worktime", "Break sum"});

            int i = 1;
            Worktimes wt = repository.FindById(i++);
            while (wt != null)
            {
                this.gui.AddRow(new[]{wt.Day, wt.StartingTime.ToString(), wt.Worktime.ToString(CultureInfo.CurrentCulture), wt.Pause.ToString()});
                wt = repository.FindById(i++);
            }
        }
    }
}