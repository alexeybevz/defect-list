using System;
using System.Linq;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System.Windows;
using DefectListDomain.CreatingReports;
using DefectListDomain.ReportParameters;
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
            try
            {
                var bomItems = await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId);
                if (_filterBomItems != null)
                    bomItems = bomItems.Where(_filterBomItems).ToList();

                var parm = new DefectListItemsRptParm()
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
                };

                var report = DefectListIocKernel.Get<IDefectListItemsReport>();
                var isReportCreated = await report.CreateAsync(
                    _bomItemViewModel.BomHeader,
                    parm,
                    _bomItemViewModel.UserIdentity.Name);

                if (isReportCreated)
                    MessageBox.Show("Отчет сформирован.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
