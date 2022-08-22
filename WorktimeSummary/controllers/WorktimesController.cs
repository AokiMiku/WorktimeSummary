namespace WorktimeSummary.controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using data;
    using repositories;

    public class WorktimesController
    {
        private WorktimesRepository repository = new WorktimesRepository();
        private MainWindow gui;

        public WorktimesController(MainWindow gui)
        {
            this.gui = gui;
            this.gui.AddRow(new[] { "Day", "Starting Time", "Worktime", "Break sum" }, true);

            List<Worktimes> wts = repository.FindAll();
            foreach (Worktimes wt in wts)
            {
                this.gui.AddRow(new[]
                {
                    wt.Day, wt.StartingTime.ToString(), wt.Worktime.ToString(CultureInfo.CurrentCulture),
                    (wt.Pause / 60).ToString()
                });
            }
            this.gui.RepaintTable();
        }
    }
}