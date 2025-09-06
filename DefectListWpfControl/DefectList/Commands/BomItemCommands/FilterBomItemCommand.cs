using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class FilterBomItemCommand : CommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public FilterBomItemCommand(DefectListItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override void Execute(object parameter = null)
        {
            _bomItemViewModel.FilterString = _bomItemViewModel.FilterString;
        }
    }
}