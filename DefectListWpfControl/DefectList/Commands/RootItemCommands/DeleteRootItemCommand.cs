using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.RootItemCommands
{
    public class DeleteRootItemCommand : AsyncCommandBase
    {
        private readonly ChoiceRootItemViewModel _choiceRootItemViewModel;
        private readonly RootItemsStore _rootItemsStore;

        public DeleteRootItemCommand(ChoiceRootItemViewModel choiceRootItemViewModel, RootItemsStore rootItemsStore)
        {
            _choiceRootItemViewModel = choiceRootItemViewModel;
            _rootItemsStore = rootItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (_choiceRootItemViewModel.SelectedRootItemViewModel == null)
                return;

            if (MessageBox.Show($"Вы действительно хотите удалить ремонтное изделие '{_choiceRootItemViewModel.SelectedRootItemViewModel.RootItem.Izdel}' " +
                                "из списка доступных изделий для дефектовочной ведомости?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            _choiceRootItemViewModel.ErrorMessage = null;

            try
            {
                await _rootItemsStore.Delete(_choiceRootItemViewModel.SelectedRootItemViewModel.RootItem.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
            }
        }
    }
}