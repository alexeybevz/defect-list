using System;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.DefectToDecisionMapCommands
{
    public class EditDefectToDecisionMapCommand : AsyncCommandBase
    {
        private readonly EditDefectToDecisionMapViewModel _editDefectToDecisionMapViewModel;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        public EditDefectToDecisionMapCommand(EditDefectToDecisionMapViewModel editDefectToDecisionMapViewModel, DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _editDefectToDecisionMapViewModel = editDefectToDecisionMapViewModel;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var formViewModel = _editDefectToDecisionMapViewModel.DefectToDecisionMapDetailsFormViewModel;

            formViewModel.ErrorMessage = null;
            formViewModel.IsSubmitting = true;

            try
            {
                var mapDefectToDecision = new MapDefectToDecision()
                {
                    Id = _editDefectToDecisionMapViewModel.MapDefectToDecision.Id,
                    Defect = formViewModel.Defect,
                    Decision = formViewModel.Decision,
                    IsAllowCombine = formViewModel.IsAllowCombine,
                    StateDetals = formViewModel.StateDetals,
                    GroupDefect = formViewModel.GroupDefect
                };

                await _defectToDecisionMapsStore.Update(mapDefectToDecision);
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