using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SetFinalDecisionCommand : AsyncCommandBase
    {
        private readonly BomItemViewModel _bomItemViewModel;
        private List<string> _decisionsTransferedToFinalDecision;

        public SetFinalDecisionCommand(BomItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
            _decisionsTransferedToFinalDecision = new List<string> {"заменить", "ремонтное воздействие не требуется", "скомплектовать", "использовать"};
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _bomItemViewModel.FinalDecision = _decisionsTransferedToFinalDecision.Contains(_bomItemViewModel.Decision) ? _bomItemViewModel.Decision : null;
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsWriteAccessUser;
        }
    }
}