using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.CreatingReports;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ScrapItemsReportCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;

        public ScrapItemsReportCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var bomItems = (await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId)).ToList().Where(x => x.IsScrap).ToList();
                if (!bomItems.Any())
                {
                    MessageBox.Show("Отчет не сформирован. Нет позиций с решением по устранению равным 'заменить'.");
                    return;
                }

                var report = DefectListIocKernel.Get<IDefectListScrapItemsReport>();
                var isReportCreated = await report.CreateAsync(_bomItemViewModel.BomHeader, bomItems, _bomItemViewModel.UserIdentity?.Name);

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