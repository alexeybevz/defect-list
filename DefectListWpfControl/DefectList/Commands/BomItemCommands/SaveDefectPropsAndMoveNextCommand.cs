using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SaveDefectPropsAndMoveNextCommand : SaveCommandBase
    {
        public SaveDefectPropsAndMoveNextCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator) : base(bomItemViewModel, bomItemsStore, saveBomItemCommand, bomItemsValidator)
        {
        }

        protected override async Task SaveAsync(object parameter)
        {
            await UpdateDefectPropsAndMoveNext(false);
        }
    }
}