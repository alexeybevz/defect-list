using System;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.RootItemCommands
{
    public class LoadRootItemsCommand : AsyncCommandBase
    {
        private readonly ChoiceRootItemViewModel _choiceRootItemViewModel;
        private readonly RootItemsStore _rootItemsStore;

        public LoadRootItemsCommand(ChoiceRootItemViewModel choiceRootItemViewModel, RootItemsStore rootItemsStore)
        {
            _choiceRootItemViewModel = choiceRootItemViewModel;
            _rootItemsStore = rootItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _choiceRootItemViewModel.ErrorMessage = null;
            _choiceRootItemViewModel.IsLoading = true;

            try
            {
                await _rootItemsStore.Load();
            }
            catch (Exception ex)
            {
                _choiceRootItemViewModel.ErrorMessage = $"Возникла ошибка при получении номенклатуры из базы данных: {ex.Message}";
            }
            finally
            {
                _choiceRootItemViewModel.IsLoading = false;
            }
        }
    }
}