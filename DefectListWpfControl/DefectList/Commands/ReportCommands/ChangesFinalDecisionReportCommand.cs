using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListBusinessLogic.Report;
using DefectListWpfControl.DefectList.Stores;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.HelpersWindow;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ChangesFinalDecisionReportCommand : AsyncCommandBase
    {
        private readonly BomItemsStore _bomItemsStore;
        private readonly string _userName;

        public ChangesFinalDecisionReportCommand(BomItemsStore bomItemsStore, string userName)
        {
            _bomItemsStore = bomItemsStore;
            _userName = userName;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddSeconds(-1);

                var window = new ChoiceDateWindow(firstDayOfMonth, lastDayOfMonth);
                var dialogResult = window.ShowDialog();

                if (dialogResult.HasValue && !dialogResult.Value)
                    return;

                var selectedStartDate = window.StartDate;
                var selectedEndDate = window.EndDate;

                IReportDirectory reportDirectory = new ReportDirectory(_userName);
                reportDirectory.Create();

                var pathToFile = reportDirectory.PathReportDirectory +
                                 $@"\Журнал изменения окончательного решения от {DateTime.Now:yyyy-MM-dd HH-mm-ss}.xlsx";

                var reportBuilder = DefectListIocKernel.Get<ChangesFinalDecisionReport>();
                var data = (await _bomItemsStore.GetFinalDecisionChangings(selectedStartDate, selectedEndDate)).ToList();
                reportBuilder.Create(data, pathToFile);

                MessageBox.Show("Отчет сформирован.");
                reportDirectory.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}