using System.Linq;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class ExpandBomItemCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly BomItemsStore _bomItemsStore;
        private readonly bool _isExpandNodeToAllLevels;

        public ExpandBomItemCommand(DefectListItemViewModel bomItemViewModel, BomItemsStore bomItemsStore, bool isExpandNodeToAllLevels)
        {
            _bomItemViewModel = bomItemViewModel;
            _bomItemsStore = bomItemsStore;
            _isExpandNodeToAllLevels = isExpandNodeToAllLevels;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            var assemblyBom = (await _bomItemsStore.GetBomItemIsDatabaseView(_bomItemViewModel.BomHeader.BomId)).ToList()
                .Where(x => x.StructureNumber.StartsWith(_bomItemViewModel.SelectedBomItemViewModel.StructureNumber)).ToList();
            await _bomItemsStore.Expand(_bomItemViewModel.SelectedBomItemViewModel.Id, _bomItemViewModel.SelectedBomItemViewModel.StructureNumber, assemblyBom, _isExpandNodeToAllLevels);
            await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsWriteAccessUser;
        }
    }
}