using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Commands;
using DefectListDomain.Services;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SaveDefectPropsOnSelectedBomItemsCommand : SaveCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public SaveDefectPropsOnSelectedBomItemsCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator) : base(bomItemViewModel, bomItemsStore, saveBomItemCommand, bomItemsValidator)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        protected override async Task SaveAsync(object parameter)
        {
            _bomItemViewModel.SelectedBomItemViewModel = (BomItemViewModel)_bomItemViewModel.BomItemsView.CurrentItem;
            if (_bomItemViewModel.SelectedBomItemViewModel == null)
                return;

            var tempDefect = _bomItemViewModel.SelectedBomItemViewModel.Defect;
            var tempDecision = _bomItemViewModel.SelectedBomItemViewModel.Decision;
            var tempResearchAction = _bomItemViewModel.SelectedBomItemViewModel.ResearchAction;
            var tempResearchResult = _bomItemViewModel.SelectedBomItemViewModel.ResearchResult;

            var dialogResult = MessageBox.Show("Дефект и решение будут применены ко всем строкам, которые отмечены галочкой. Вы уверены?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            var dialogResult2 = dialogResult == MessageBoxResult.Yes ? MessageBox.Show("Вы действительно хотите применить дефект и решение ко всем строкам, которые отмечены галочкой?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) : MessageBoxResult.No;

            if (dialogResult == MessageBoxResult.Yes && dialogResult2 == MessageBoxResult.Yes)
                if (await ApplyTempDefectAndDecision(x => x.IsSelected, tempDefect, tempDecision, tempResearchAction, tempResearchResult))
                    return;

            await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();
            MessageBox.Show("Готово");
        }
    }
}