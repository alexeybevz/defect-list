using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System;
using DefectListDomain.Models;

namespace DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands
{
    public class AddDefectToDecisionMapCommand : AsyncCommandBase
    {
        private readonly AddDefectToDecisionMapViewModel _addDefectToDecisionMapViewModel;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        public AddDefectToDecisionMapCommand(AddDefectToDecisionMapViewModel addDefectToDecisionMapViewModel, DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _addDefectToDecisionMapViewModel = addDefectToDecisionMapViewModel;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var formViewModel = _addDefectToDecisionMapViewModel.DefectToDecisionMapDetailsFormViewModel;

            formViewModel.ErrorMessage = null;
            formViewModel.IsSubmitting = true;

            try
            {
                var mapDefectToDecision = new MapDefectToDecision()
                {
                    Defect = formViewModel.Defect,
                    Decision = formViewModel.Decision,
                    IsAllowCombine = formViewModel.IsAllowCombine,
                    StateDetals = formViewModel.StateDetals,
                    GroupDefect = formViewModel.GroupDefect
                };

                await _defectToDecisionMapsStore.Add(mapDefectToDecision);
            }
            catch (Exception ex)
            {
                formViewModel.ErrorMessage = "Ошибка: " + ex.Message;
            }
            finally
            {
                formViewModel.IsSubmitting = false;
            }
        }
    }
}