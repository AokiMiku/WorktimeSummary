namespace WorktimeSummary.controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using data;
    using repositories;

    public class WorktimesController
    {
        private readonly WorktimesRepository repository = new WorktimesRepository();
        private readonly MainWindow gui;

        public WorktimesController(MainWindow gui)
        {
            this.gui = gui;
            FillData();
            this.gui.RepaintTable();
        }

        private void FillData()
        {
            gui.AddRow(new[] { "Day", "Starting Time", "Worktime", "Break sum" }, true);

            List<Worktimes> wts = repository.FindAll();
            foreach (Worktimes wt in wts)
            {
                gui.AddRow(new[]
                {
                    wt.Day, wt.StartingTime.ToString(), wt.Worktime.ToString(CultureInfo.CurrentCulture),
                    (wt.Pause / 60).ToString()
                });
            }
        }
    }
}