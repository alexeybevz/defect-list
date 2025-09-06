using System.Linq;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class CollapseBomItemCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;

        public CollapseBomItemCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var assemblyBom = (await _bomItemsStore.GetBomItemIsDatabaseView(_bomItemViewModel.BomHeader.BomId)).ToList()
                .Where(x => x.StructureNumber.StartsWith(_bomItemViewModel.SelectedBomItemViewModel.StructureNumber)).ToList();
            await _bomItemsStore.Collapse(_bomItemViewModel.SelectedBomItemViewModel.Id, assemblyBom);
            await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsWriteAccessUser;
        }
    }
}