using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class ClearSearchBomItemCommand : CommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public ClearSearchBomItemCommand(DefectListItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override void Execute(object parameter = null)
        {
            _bomItemViewModel.SearchString = string.Empty;
        }
    }
}