using System;
using System.Linq;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.CreatingReports;
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
            try
            {
                var bomHeader = _bomItemViewModel.BomHeader;
                var bomItems = await _bomItemsStore.GetBomItemIsShowedView(bomHeader.BomId);
                var bomItemLogs = await _bomItemsStore.GetBomItemLogs(bomHeader.BomId);

                var report = DefectListIocKernel.Get<IDefectListItemsChangesReport>();
                var isReportCreated = await report.CreateAsync(bomHeader, bomItems.ToList(), bomItemLogs.ToList(), _bomItemViewModel.UserIdentity.Name);

                if (isReportCreated)
                    MessageBox.Show("Отчет сформирован.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}