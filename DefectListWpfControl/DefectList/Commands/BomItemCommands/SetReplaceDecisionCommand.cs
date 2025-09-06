using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SetReplaceDecisionCommand : CommandBase
    {
        private readonly BomItemViewModel _bomItemViewModel;

        public SetReplaceDecisionCommand(BomItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override void Execute(object parameter = null)
        {
            _bomItemViewModel.Decision = "заменить";
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsWriteAccessUser &&
                   (_bomItemViewModel?.IsValid ?? false);
        }
    }
}