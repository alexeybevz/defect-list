using System;
using System.Linq;
using System.Threading.Tasks;
using DefectListBusinessLogic.Report;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System.Windows;
using DefectListDomain.ReportParameters;
using DefectListDomain.Reports;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ExportDefectListToPdfCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly bool _isUseFinalDecision;
        private readonly Func<BomItem, bool> _filterBomItems;

        public ExportDefectListToPdfCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore, bool isUseFinalDecision, Func<BomItem, bool> filterBomItems = null)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _isUseFinalDecision = isUseFinalDecision;
            _filterBomItems = filterBomItems;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            IReportDirectory reportDirectory = new ReportDirectory(_bomItemViewModel.UserIdentity.Name);

            try
            {
                reportDirectory.Create();

                var bomItems = await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId);
                if (_filterBomItems != null)
                    bomItems = bomItems.Where(_filterBomItems).ToList();

                var reportParmBuilder = new DefectListItemsRptParmBuilder(new DefectListItemsRptParm()
                {
                    BomId = _bomItemViewModel.BomHeader.BomId,
                    Izdel = _bomItemViewModel.BomHeader.RootItem.Izdel,
                    IzdelInitial = _bomItemViewModel.BomHeader.RootItem.IzdelInitial,
                    SerialNumber = _bomItemViewModel.BomHeader.SerialNumber,
                    SerialNumberAfterRepair = _bomItemViewModel.BomHeader.SerialNumberAfterRepair,
                    Contract = _bomItemViewModel.BomHeader.Contract,
                    ContractDateOpen = _bomItemViewModel.BomHeader.ContractDateOpen,
                    IsUseFinalDecision = _isUseFinalDecision,
                    DateOfPreparation = _bomItemViewModel.BomHeader.DateOfPreparation ?? DateTime.Now.Date,
                    BomItems = bomItems
                });

                var reportBuilder = new DefectListItemsRptBuilder(reportParmBuilder);
                var reportSender = new CrRptPdfSender<DefectListItems>(reportDirectory.PathReportDirectory);
                var reporter = new Reporter<DefectListItems>(reportBuilder, reportSender);

                var countReports = reporter.SendReports();
                MessageBox.Show($"Создано отчетов: {countReports}");

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
