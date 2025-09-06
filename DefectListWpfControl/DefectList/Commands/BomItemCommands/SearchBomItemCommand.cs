using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SearchBomItemCommand : CommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public SearchBomItemCommand(DefectListItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override void Execute(object parameter = null)
        {
            _bomItemViewModel.SearchString = _bomItemViewModel.SearchString;
        }
    }
}