using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using DefectListDomain.CreatingReports;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ExportDefectListToExcelCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;

        public ExportDefectListToExcelCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var bomItems = (await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId)).ToList();
                if (!bomItems.Any())
                {
                    MessageBox.Show("Отчет не сформирован. Нет данных.");
                    return;
                }

                var reportBuilder = DefectListIocKernel.Get<IDefectListAllItemsReport>();
                var isReportCreated = await reportBuilder.CreateAsync(_bomItemViewModel.BomHeader, bomItems, _bomItemViewModel.UserIdentity.Name);

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
