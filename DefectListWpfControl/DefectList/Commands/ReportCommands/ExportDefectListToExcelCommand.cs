using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using DefectListBusinessLogic.Report;
using DefectListDomain.ExternalData;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ExportDefectListToExcelCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly IGetAllPlanOperationDtoQuery _getAllPlanOperationDtoQuery;

        public ExportDefectListToExcelCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore, IGetAllPlanOperationDtoQuery getAllPlanOperationDtoQuery)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _getAllPlanOperationDtoQuery = getAllPlanOperationDtoQuery;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            IReportDirectory reportDirectory = new ReportDirectory(_bomItemViewModel.UserIdentity.Name);

            try
            {
                reportDirectory.Create();

                var bomItems = (await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId)).ToList();
                if (!bomItems.Any())
                {
                    MessageBox.Show("Отчет не сформирован. Нет данных.");
                    return;
                }

                var reportBuilder = new DefectListAllItemsReport(_getAllPlanOperationDtoQuery);
                reportBuilder.Create(_bomItemViewModel.BomHeader, bomItems, reportDirectory.PathReportDirectory);

                MessageBox.Show("Отчет сформирован.");
                reportDirectory.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                reportDirectory.DeleteIfEmpty();
            }
        }
    }
}
