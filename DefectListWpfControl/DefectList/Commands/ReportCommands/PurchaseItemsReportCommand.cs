using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListBusinessLogic.Report;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class PurchaseItemsReportCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly BomHeadersStore _bomHeadersStore;

        public PurchaseItemsReportCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            BomHeadersStore bomHeadersStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _bomHeadersStore = bomHeadersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            IReportDirectory reportDirectory = new ReportDirectory(_bomItemViewModel.UserIdentity.Name);
            try
            {
                reportDirectory.Create();

                var filterBomItems = await _bomHeadersStore.GetAllBomItemsFilterToReport(_bomItemViewModel.BomHeader.RootItem.Id, "PurchaseItemsReport");

                var bomItems = (await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId)).ToList().Where(x => x.IsPki && filterBomItems.Contains(x.Detal.Trim())).ToList();
                if (!bomItems.Any())
                {
                    MessageBox.Show("Отчет не сформирован. Нет данных.");
                    return;
                }

                var reportBuilder = new DefectListPurchaseItemsReport();
                reportBuilder.Create(_bomItemViewModel.BomHeader, bomItems, reportDirectory.PathReportDirectory);

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
