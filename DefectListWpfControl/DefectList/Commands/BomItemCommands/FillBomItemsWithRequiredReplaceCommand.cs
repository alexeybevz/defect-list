using System;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using System.Linq;
using DefectListDomain.Commands;
using DefectListDomain.Services;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class FillBomItemsWithRequiredReplaceCommand : SaveCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public FillBomItemsWithRequiredReplaceCommand(
            DefectListItemViewModel bomItemViewModel,
            BomItemsStore bomItemsStore,
            ISaveBomItemCommand saveBomItemCommand,
            IBomItemsValidator bomItemsValidator): base(bomItemViewModel, bomItemsStore, saveBomItemCommand, bomItemsValidator)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        protected override async Task SaveAsync(object parameter = null)
        {
            try
            {
                var dialogResult = MessageBox.Show("Вы действительно хотите применить к целевым ДСЕ связку дефект/решение: разового применения/заменить?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dialogResult == MessageBoxResult.No)
                    return;

                var targetMapDefectToDecision = _bomItemViewModel
                    .DefectToDecisionMaps.OfType<DefectToDecisionMapCheckBoxViewModel>()
                    .FirstOrDefault(x => x.Item.Defect == "разового применения");

                if (targetMapDefectToDecision == null)
                {
                    MessageBox.Show("Не найдена связка дефект/решение. Действие отменено");
                    return;
                }

                var bomItems = _bomItemViewModel
                    .BomItemsView
                    .OfType<BomItemViewModel>()
                    .Where(x => x.RepairMethodName == "обязательная замена" && !x.IsFilled)
                    .ToList();

                foreach (var item in bomItems)
                {
                    _bomItemViewModel.SelectedBomItemViewModel = item;
                    _bomItemViewModel.MapDefectToDecisionChanged(targetMapDefectToDecision, true);
                    await UpdateDefectProps(item);
                }

                await _bomItemViewModel.LoadBomItemsCommand.ExecuteAsync();
                MessageBox.Show("Готово");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && 
                   PermissionsStore.IsSuperUser &&
                   !_bomItemViewModel.BomHeader.StateInfo.IsClosed;
        }
    }
}