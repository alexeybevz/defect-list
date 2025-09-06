using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class CreateRouteChartCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public CreateRouteChartCommand(DefectListItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var selectedBomItems = new List<BomItemViewModel>() {_bomItemViewModel.SelectedBomItemViewModel};
                var form = await ConsolidateBomItemsWindow.CreateAsync(_bomItemViewModel.BomHeader, selectedBomItems);
                var vm = (ConsolidateBomItemsViewModel)form.DataContext;

                vm.CreateRouteChartsCommand.Execute(null);

                var result = vm.Rows.First();

                await _bomItemViewModel.LoadBomItemCommand.ExecuteAsync();

                if (!result.IsExistsTargetDetal)
                    result.Comment = "Отсутствует ДСЕ " + result.TargetDetal;

                if (string.IsNullOrEmpty(result.CreatedRouteMap))
                {
                    _bomItemViewModel._errorMessage.Text = "Не удалось создать МК: " + result.Comment;
                    _bomItemViewModel.InfoMessage = _bomItemViewModel._errorMessage;
                }
                else
                {
                    _bomItemViewModel.InfoMessage = _bomItemViewModel._successMessage;
                }
            }
            catch (Exception e)
            {
                _bomItemViewModel._errorMessage.Text = e.Message;
                _bomItemViewModel.InfoMessage = _bomItemViewModel._errorMessage;
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) &&
                   PermissionsStore.IsCanCreateRouteMapsUser &&
                   !_bomItemViewModel.BomHeader.StateInfo.IsClosed &&
                   _bomItemViewModel.SelectedBomItemViewModel != null &&
                   BomItemsConstantsStore.DecisionsToCreateRouteMap.Contains(_bomItemViewModel.SelectedBomItemViewModel?.Decision);
        }
    }
}