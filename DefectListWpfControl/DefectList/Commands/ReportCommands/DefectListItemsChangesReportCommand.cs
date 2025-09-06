using System;
using System.Linq;
using DefectListBusinessLogic.Report;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class DefectListItemsChangesReportCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;

        public DefectListItemsChangesReportCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            IReportDirectory reportDirectory = new ReportDirectory(_bomItemViewModel.UserIdentity.Name);
            try
            {
                reportDirectory.Create();

                var bomHeader = _bomItemViewModel.BomHeader;
                var bomItems = await _bomItemsStore.GetBomItemIsShowedView(bomHeader.BomId);
                var bomItemLogs = await _bomItemsStore.GetBomItemLogs(bomHeader.BomId);

                var defectListItemsChangesReport = new DefectListItemsChangesReport();
                defectListItemsChangesReport.Create(bomHeader, bomItems.ToList(), bomItemLogs.ToList(), reportDirectory.PathReportDirectory);

                MessageBox.Show("Отчет сформирован.");
                reportDirectory.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                reportDirectory.DeleteIfEmpty();
            }
        }
    }
}