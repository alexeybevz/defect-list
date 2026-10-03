using System.Collections.Generic;
using System.Linq;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class DefectSelectionChangedCommand
    {
        private readonly DefectListItemViewModel _vm;

        public DefectSelectionChangedCommand(DefectListItemViewModel vm)
        {
            _vm = vm;
        }

        public bool Handle(DefectToDecisionMapCheckBoxViewModel item, bool newValueIsSelected)
        {
            if (_vm.SelectedBomItemViewModel == null)
                return false;

            var selectedItems = GetSelectedDefectItems();

            if (newValueIsSelected)
                return SelectDefect(item, selectedItems);

            UnselectDefect(item);

            return false;
        }

        private List<DefectToDecisionMapCheckBoxViewModel> GetSelectedDefectItems()
        {
            return _vm.DefectToDecisionMaps
              .OfType<DefectToDecisionMapCheckBoxViewModel>()
              .Where(x => x.IsSelected)
              .ToList();
        }

        private bool SelectDefect(
          DefectToDecisionMapCheckBoxViewModel item,
          List<DefectToDecisionMapCheckBoxViewModel> selectedItems)
        {
            // IsAllowCombine = false означает самостоятельный выбор:
            // такой дефект нельзя комбинировать с другими дефектами.
            if (!CanSelectDefect(item, selectedItems))
                return false;

            var defect = item.Item.Defect;
            var currentDefect = _vm.SelectedBomItemViewModel.Defect;
            var isEmpty = string.IsNullOrEmpty(currentDefect);

            _vm.SelectedBomItemViewModel.Defect = isEmpty
              ? defect
              : currentDefect + ", " + defect;

            // Комбинируемые дефекты по бизнес-правилам имеют одно Decision.
            // Поэтому при добавлении берём Decision уже выбранных дефектов.
            _vm.SelectedBomItemViewModel.Decision =
            isEmpty || !selectedItems.Any()
              ? item.Item.Decision
              : selectedItems
                .Select(x => x.Item.Decision)
                .Distinct()
                .FirstOrDefault();

            return true;
        }

        private bool CanSelectDefect(
          DefectToDecisionMapCheckBoxViewModel item,
          List<DefectToDecisionMapCheckBoxViewModel> selectedItems)
        {
            // Первый дефект можно выбрать независимо от IsAllowCombine.
            if (!selectedItems.Any())
                return true;

            // При наличии выбранных дефектов все они должны поддерживать
            // комбинирование, и новый дефект также должен его поддерживать.
            return item.Item.IsAllowCombine &&
             selectedItems.All(x => x.Item.IsAllowCombine);
        }

        private void UnselectDefect(
          DefectToDecisionMapCheckBoxViewModel item)
        {
            var currentDefect = _vm.SelectedBomItemViewModel.Defect;

            if (string.IsNullOrEmpty(currentDefect))
                return;

            var defect = item.Item.Defect;

            // Если снимается единственный дефект — очищаем результат
            // и связанные с ним количества.
            if (currentDefect == defect)
            {
                ClearDefect();
                return;
            }

            if (currentDefect.StartsWith(defect))
            {
                _vm.SelectedBomItemViewModel.Defect =
                  currentDefect.Remove(0, defect.Length + 2);

                return;
            }

            var position = currentDefect.IndexOf(defect);

            if (position >= 0)
            {
                _vm.SelectedBomItemViewModel.Defect =
                  currentDefect.Remove(position - 2, defect.Length + 2);
            }
        }

        private void ClearDefect()
        {
            _vm.SelectedBomItemViewModel.Defect = null;
            _vm.SelectedBomItemViewModel.Decision = null;
            _vm.SelectedBomItemViewModel.QtyRestore = 0;
            _vm.SelectedBomItemViewModel.QtyReplace = 0;
        }
    }
}