using System;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomHeaderCommands
{
    public class LoadBomHeadersCommand : AsyncCommandBase
    {
        private readonly DefectListHeaderViewModel _bomHeaderViewModel;
        private readonly BomHeadersStore _bomHeadersStore;

        public LoadBomHeadersCommand(DefectListHeaderViewModel bomHeaderViewModel, BomHeadersStore bomHeadersStore)
        {
            _bomHeaderViewModel = bomHeaderViewModel;
            _bomHeadersStore = bomHeadersStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _bomHeaderViewModel.ErrorMessage = null;
            _bomHeaderViewModel.IsLoading = true;

            try
            {
                await _bomHeadersStore.Load();
            }
            catch (Exception e)
            {
                _bomHeaderViewModel.ErrorMessage = "Ошибка при загрузке дефектовочных ведомостей. Пожалуйста, перезагрузите программу.";
            }
            finally
            {
                _bomHeaderViewModel.IsLoading = false;
            }
        }
    }
}