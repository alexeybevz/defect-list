using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.DefectList.Views;
using DefectListWpfControl.ViewModelImplement;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class OpenCreateRouteChartsFormCommand : AsyncCommandBase
    {
        private readonly DefectListItemViewModel _bomItemViewModel;

        public OpenCreateRouteChartsFormCommand(DefectListItemViewModel bomItemViewModel)
        {
            _bomItemViewModel = bomItemViewModel;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var selectedBomItems = _bomItemViewModel.BomItemsView
                    .OfType<BomItemViewModel>()
                    .Where(x => x.IsSelected)
                    .OrderBy(x => x.Detal)
                    .ToList();

                var form = await ConsolidateBomItemsWindow.CreateAsync(_bomItemViewModel.BomHeader, selectedBomItems);
                form.ShowDialog();
                _bomItemViewModel.LoadBomItemsCommand?.Execute();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public override bool CanExecute(object parameter = null)
        {
            return base.CanExecute(parameter) && PermissionsStore.IsCanCreateRouteMapsUser && !_bomItemViewModel.BomHeader.StateInfo.IsClosed;
        }
    }
}