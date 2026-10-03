using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.MeasurementMapDictionaryCommands
{
    public class DeleteDictionaryItemCommand : AsyncCommandBase
    {
        private readonly LoadingStateViewModel _loadingState;
        private readonly MeasurementMapDictionaryStore _store;
        private readonly MeasurementMapDictionaryItemsViewModel _vm;

        public DeleteDictionaryItemCommand(
            LoadingStateViewModel loadingState,
            MeasurementMapDictionaryStore store,
            MeasurementMapDictionaryItemsViewModel vm)
        {
            _loadingState = loadingState;
            _store = store;
            _vm = vm;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var selected = _vm.SelectedItem;
            if (selected == null) return;

            var confirm = MessageBox.Show(
                $"Удалить строку «{selected.CustomNumeration} {selected.PossibleDefectName}»?\n" +
                "Удаление невозможно, если строка используется в экземплярах карт измерений.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            await _loadingState.ExecuteWithLoadingAsync(async () =>
            {
                await _store.DeleteItemAsync(selected.Model, _vm.User?.Name);
            });
        }

        public override bool CanExecute(object parameter = null)
        {
            return _vm.SelectedItem != null &&
                   _vm.IsDictionaryActive;
        }
    }
}