using System.Linq;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SetRestoreReplaceQtyCommand : AsyncCommandBase
    {
        private readonly BomItemViewModel _bomItemViewModel;
        private readonly DefectToDecisionMapsStore _defectToDecisionMapsStore;

        public SetRestoreReplaceQtyCommand(
            BomItemViewModel bomItemViewModel,
            DefectToDecisionMapsStore defectToDecisionMapsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _defectToDecisionMapsStore = defectToDecisionMapsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var list = _defectToDecisionMapsStore
                .DefectToDecisionMaps
                .Select(x => new { x.Decision, x.StateDetals.StateDetalsName })
                .Distinct();

            var defectState = list.FirstOrDefault(x => x.Decision == _bomItemViewModel.Decision)?.StateDetalsName ?? "Годная";
            if (defectState == "Ремонт")
            {
                _bomItemViewModel.QtyRestore = _bomItemViewModel.QtyMnf;
                _bomItemViewModel.QtyReplace = 0;
            }

            if (defectState == "Замена")
            {
                _bomItemViewModel.QtyRestore = 0;
                _bomItemViewModel.QtyReplace = _bomItemViewModel.QtyMnf;
            }

            if (defectState == "Годная")
            {
                _bomItemViewModel.QtyRestore = 0;
                _bomItemViewModel.QtyReplace = 0;
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsWriteAccessUser;
        }
    }
}