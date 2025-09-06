using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SaveDefectPropsOnAssemblyWithNotFilledRowsCommand : SaveCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public SaveDefectPropsOnAssemblyWithNotFilledRowsCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator) : base(bomItemViewModel, bomItemsStore, saveBomItemCommand, bomItemsValidator)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        protected override async Task SaveAsync(object parameter)
        {
            await UpdateDefectPropsAndMoveNext(true, _bomItemViewModel.FilterByStructureNumberWithNotFilledRows());
        }
    }
}