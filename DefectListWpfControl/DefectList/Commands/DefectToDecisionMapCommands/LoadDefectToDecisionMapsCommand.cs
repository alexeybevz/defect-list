using System;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands
{
    public class LoadDefectToDecisionMapsCommand : AsyncCommandBase
    {
        private readonly DefectToDecisionMapsViewModel _mapDefectToDecisionViewModel;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        public LoadDefectToDecisionMapsCommand(DefectToDecisionMapsViewModel mapDefectToDecisionViewModel, DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _mapDefectToDecisionViewModel = mapDefectToDecisionViewModel;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _mapDefectToDecisionViewModel.ErrorMessage = null;
            _mapDefectToDecisionViewModel.IsLoading = true;

            try
            {
                await _defectToDecisionMapsStore.Load();
            }
            catch (Exception e)
            {
                _mapDefectToDecisionViewModel.ErrorMessage = "Ошибка при загрузке связок дефект-решение. Пожалуйста, перезагрузите форму.";
            }
            finally
            {
                _mapDefectToDecisionViewModel.IsLoading = false;
            }
        }
    }
}