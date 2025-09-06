using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class ClearFilterBomItemCommand : CommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public ClearFilterBomItemCommand(DefectListItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override void Execute(object parameter = null)
        {
            _bomItemViewModel.FilterString = string.Empty;
        }
    }
}