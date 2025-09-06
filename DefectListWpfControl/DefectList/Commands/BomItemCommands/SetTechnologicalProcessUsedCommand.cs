using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SetTechnologicalProcessUsedCommand : AsyncCommandBase
    {
        private readonly BomItemViewModel _bomItemViewModel;

        public SetTechnologicalProcessUsedCommand(BomItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            _bomItemViewModel.UpdateTehprocHeader();
            _bomItemViewModel.TechnologicalProcessUsed = _bomItemViewModel.TehprocHeader.Value;
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsWriteAccessUser;
        }
    }
}