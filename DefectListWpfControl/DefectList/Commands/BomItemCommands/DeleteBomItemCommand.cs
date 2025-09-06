using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class DeleteBomItemCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;
        private readonly SelectedBomItemStore _selectedBomItemStore;
        private readonly BomItemsStore _bomItemsStore;

        public DeleteBomItemCommand(
            DefectListItemViewModel bomItemViewModel,
            SelectedBomItemStore selectedBomItemStore,
            BomItemsStore bomItemsStore)
        {
            _bomItemViewModel = bomItemViewModel;
            _selectedBomItemStore = selectedBomItemStore;
            _bomItemsStore = bomItemsStore;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                List<BomItem> deletedBomItems = null;
                if (_selectedBomItemStore.SelectedBomItem.UzelFlag == 1)
                {
                    deletedBomItems = _bomItemViewModel.BomItemsView.OfType<BomItemViewModel>()
                        .Where(x => x.StructureNumber.StartsWith(_selectedBomItemStore.SelectedBomItem.StructureNumber)).Select(x => x.BomItem).ToList();
                }
                else
                {
                    deletedBomItems = new List<BomItem>() { _selectedBomItemStore.SelectedBomItem };
                }

                var dialogResult = MessageBox.Show("Вы действительно хотите удалить ДСЕ из состава?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dialogResult == MessageBoxResult.No)
                    return;

                if (!deletedBomItems.Any())
                    throw new ArgumentException("Нет данных для выполнения операции");

                _bomItemsStore.Delete(_bomItemViewModel.BomHeader, _selectedBomItemStore.SelectedBomItem, deletedBomItems, _bomItemViewModel.UserIdentity.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("При удалении произошла ошибка: " + ex.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsSuperUser && (_bomItemViewModel.BomHeader.StateInfo.IsWip || _bomItemViewModel.BomHeader.StateInfo.IsWaitApproved);
        }
    }
}