using System;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class LoadBomItemsCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;
        private readonly TehprocHeadersStore _tehprocHeadersStore;

        public LoadBomItemsCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            DefectToDecisionMapsStore defectToDecisionMapsStore,
            TehprocHeadersStore tehprocHeadersStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
            _tehprocHeadersStore = tehprocHeadersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _bomItemViewModel.IsLoading = true;
            _bomItemViewModel.LoadingErrorMessage = string.Empty;

            try
            {
                await _defectToDecisionMapsStore.Load();
                await _tehprocHeadersStore.Load();
                await _bomItemViewModel.RefreshDefectListHeader();
                await _bomItemsStore.Load(_bomItemViewModel.BomHeader.BomId);
            }
            catch (Exception e)
            {
                _bomItemViewModel.LoadingErrorMessage = "Ошибка при загрузке дефектовочных ведомостей. Пожалуйста, перезагрузите программу.";
            }
            finally
            {
                _bomItemViewModel.IsLoading = false;
            }
        }
    }
}