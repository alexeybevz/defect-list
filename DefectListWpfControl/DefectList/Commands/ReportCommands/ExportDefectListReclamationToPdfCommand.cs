using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListBusinessLogic.Report;
using DefectListDomain.CreatingReports;
using DefectListDomain.Models;
using DefectListDomain.ReportParameters;
using DefectListDomain.Reports;
using ReporterDomain.Services.CreateReportService;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ExportDefectListReclamationToPdfCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly Func<IBomItemModel, bool> _filterBomItems;

        public ExportDefectListReclamationToPdfCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            Func<IBomItemModel, bool> filterBomItems)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _filterBomItems = filterBomItems;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var dbBomItems = await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId);

                var bomItemViewModels = _bomItemViewModel
                    .BomItemsView.Cast<BomItemViewModel>()
                    .ToDictionary(x => x.Id);

                var selectedBomItemViewModels = bomItemViewModels
                    .Where(x => _filterBomItems(x.Value))
                    .ToDictionary(x => x.Key, v => v.Value);
                if (!selectedBomItemViewModels.Any())
                    selectedBomItemViewModels = bomItemViewModels;

                var selectedBomItems = dbBomItems.Where(x => selectedBomItemViewModels.ContainsKey(x.Id)).ToList();

                var parm = new DefectListItemsRptParm()
                {
                    BomId = _bomItemViewModel.BomHeader.BomId,
                    Izdel = _bomItemViewModel.BomHeader.RootItem.Izdel,
                    IzdelInitial = _bomItemViewModel.BomHeader.RootItem.IzdelInitial,
                    SerialNumber = _bomItemViewModel.BomHeader.SerialNumber,
                    SerialNumberAfterRepair = _bomItemViewModel.BomHeader.SerialNumberAfterRepair,
                    Contract = _bomItemViewModel.BomHeader.Contract,
                    ContractDateOpen = _bomItemViewModel.BomHeader.ContractDateOpen,
                    IsUseFinalDecision = false,
                    DateOfPreparation = _bomItemViewModel.BomHeader.DateOfPreparation ?? DateTime.Now.Date,
                    BomItems = selectedBomItems
                };

                var report = DefectListIocKernel.Get<IDefectListItemsReclamationReport>();
                var isReportCreated = await report.CreateAsync(
                    _bomItemViewModel.BomHeader,
                    parm,
                    _bomItemViewModel.UserIdentity.Name);

                _bomItemViewModel.LoadBomItemsCommand?.Execute();

                if (isReportCreated)
                    MessageBox.Show("Отчет сформирован.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && (_bomItemViewModel.BomHeader.HeaderType == "Рекламация" || _bomItemViewModel.BomHeader.RootItem.Izdel.StartsWith("АЭС40.00.00.000"));
        }
    }
}
