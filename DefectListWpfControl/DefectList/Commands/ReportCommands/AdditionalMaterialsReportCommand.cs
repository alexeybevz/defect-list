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
    public class AdditionalMaterialsReportCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;

        public AdditionalMaterialsReportCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var bomItems = (await _bomItemsStore.GetBomItemIsShowedView(_bomItemViewModel.BomHeader.BomId)).ToList();

                var reportBuilder = DefectListIocKernel.Get<IAuxiliaryMaterialsReport>();
                var isReportCreated = await reportBuilder.CreateAsync(_bomItemViewModel.BomHeader, bomItems, _bomItemViewModel.UserIdentity.Name);

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