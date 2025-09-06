using System;
using System.Linq;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class SelectBomItemsCommand : CommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly Func<IBomItemModel, bool> _filterBomItems;

        public SelectBomItemsCommand(DefectListItemViewModel bomItemViewModel, Func<IBomItemModel, bool> filterBomItems)
        {
            _bomItemViewModel = bomItemViewModel;
            _filterBomItems = filterBomItems;
        }

        public override void Execute(object parameter = null)
        {
            if (_bomItemViewModel.SelectedBomItemViewModel == null)
                return;

            var isSelect = _bomItemViewModel.SelectedBomItemViewModel.IsSelected;
            var targetBomItems = _bomItemViewModel
                .BomItemsView
                .OfType<BomItemViewModel>()
                .Where(_filterBomItems)
                .ToList();

            targetBomItems.ForEach(x => x.IsSelected = isSelect);
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && (PermissionsStore.IsWriteAccessUser || PermissionsStore.IsCanCreateRouteMapsUser);
        }
    }
}