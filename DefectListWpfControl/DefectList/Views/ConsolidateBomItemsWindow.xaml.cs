using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DefectListDomain.Commands;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListWpfControl.DefectList.ViewModels;

namespace DefectListWpfControl.DefectList.Views
{
    /// <summary>
    /// Interaction logic for ConsolidateBomItemsWindow.xaml
    /// </summary>
    public partial class ConsolidateBomItemsWindow : Window
    {
        private ConsolidateBomItemsWindow() { }

        public static async Task<ConsolidateBomItemsWindow> CreateAsync(IBomHeader bomHeader, IEnumerable<BomItemViewModel> consolidateBomItems)
        {
            var instanse = new ConsolidateBomItemsWindow();
            await instanse.InitializeAsync(bomHeader, consolidateBomItems);
            return instanse;
        }

        private async Task InitializeAsync(IBomHeader bomHeader, IEnumerable<BomItemViewModel> consolidateBomItems)
        {
            InitializeComponent();

            var viewModel = await ConsolidateBomItemsViewModel.CreateAsync(
                bomHeader,
                consolidateBomItems,
                DefectListIocKernel.Get<IRouteMapFactory>(),
                DefectListIocKernel.Get<IGetAllProductDtoQuery>(),
                DefectListIocKernel.Get<IGetAllWpDtoQuery>(),
                DefectListIocKernel.Get<IGetAllMapsBomItemToRouteChartsQuery>(),
                DefectListIocKernel.Get<ICreateMapBomItemToRouteChartCommand>());
            DataContext = viewModel;
        }

        private void DataGrid_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            var visibleColumns = ConsolidatedBomItemsDataGrid.Columns.Where(x => x.Visibility == Visibility.Visible).ToList();
            var index = visibleColumns.IndexOf(ConsolidatedBomItemsDataGrid.CurrentCell.Column);

            var currentCell = e.ClipboardRowContent[index];
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(currentCell);
        }
    }
}
