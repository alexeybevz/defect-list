using System;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class LoadBomItemCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly SelectedBomItemStore _selectedBomItemStore;
        private readonly TehprocHeadersStore _tehprocHeadersStore;

        public LoadBomItemCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore, SelectedBomItemStore selectedBomItemStore, TehprocHeadersStore tehprocHeadersStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _selectedBomItemStore = selectedBomItemStore;
            _tehprocHeadersStore = tehprocHeadersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                if (_selectedBomItemStore.SelectedBomItem == null)
                    return;

                await _bomItemViewModel.RefreshDefectListHeader();
                await _tehprocHeadersStore.LoadByDetals(_selectedBomItemStore.SelectedBomItem.Detals);
                await _bomItemsStore.LoadById(_selectedBomItemStore.SelectedBomItem.Id);
            }
            catch (Exception e)
            {
                _bomItemViewModel.LoadingErrorMessage = "Ошибка при загрузке дефектовочных ведомостей. Пожалуйста, перезагрузите программу.";
            }
            finally
            {
            }
        }
    }
}