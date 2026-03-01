using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DefectListDomain.Commands;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListWpfControl.DefectList.Commands.BomItemCommands;
using DefectListWpfControl.DefectList.Commands.ConsolidateBomItemsCommands;
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.Stores;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ConsolidateBomItemsViewModel : ViewModel
    {
        private IGetAllMapsBomItemToRouteChartsQuery _getAllMapsBomItemToRouteChartsQuery;

        public ObservableCollection<ConsolidateBomItemViewModel> Rows { get; private set; }
        public IBomHeader BomHeader { get; private set; }
        public Dictionary<int, string> Depts { get; private set; }

        public ICommand CreateRouteChartsCommand { get; private set; }
        public LoadConsolidateBomItemsCommand LoadConsolidateBomItemsCommand { get; private set; }

        private ConsolidateBomItemsViewModel() { }

        private async Task InitializeAsync(
            IBomHeader bomHeader,
            IEnumerable<BomItemViewModel> selectedBomItems,
            IRouteMapFactory routeMapFactory,
            IGetAllProductDtoQuery getAllProductDtoQuery,
            IGetAllWpDtoQuery getAllWpDtoQuery,
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
            ICreateMapBomItemToRouteChartCommand createMapBomItemToRouteChartCommand)
        {
            _getAllMapsBomItemToRouteChartsQuery = getAllMapsBomItemToRouteChartsQuery;
            BomHeader = bomHeader;

            IsReadOnlyComponent = !PermissionsStore.IsCanPdoCreateRouteMapsUser;
            IsEnabledComponent = PermissionsStore.IsCanPdoCreateRouteMapsUser;

            Depts = (await getAllWpDtoQuery.ExecuteAsync()).Where(x => x.Wp_Reporter_CreateRouteMap)
                .ToDictionary(k => k.Wp_Id, v => v.Wp_Name);

            Rows = new ObservableCollection<ConsolidateBomItemViewModel>();
            CreateRouteChartsCommand = new CreateRouteChartsCommand(this, routeMapFactory, createMapBomItemToRouteChartCommand);
            IsConsolidateBomItemsByName = !PermissionsStore.IsCanOtkCreateRouteMapsUser;

            LoadConsolidateBomItemsCommand = new LoadConsolidateBomItemsCommand(this, selectedBomItems, _getAllMapsBomItemToRouteChartsQuery, getAllProductDtoQuery);
            await LoadConsolidateBomItemsCommand?.ExecuteAsync();

            if (!Rows.Any())
                throw new Exception("Для запуска МК нет строк, соответствующих условиям");
        }

        protected override void ExecuteDispose()
        {
            ((IDisposable)LoadConsolidateBomItemsCommand).Dispose();

            base.ExecuteDispose();
        }

        public static async Task<ConsolidateBomItemsViewModel> CreateAsync(
            IBomHeader bomHeader,
            IEnumerable<BomItemViewModel> selectedBomItems,
            IRouteMapFactory routeMapFactory,
            IGetAllProductDtoQuery getAllProductDtoQuery,
            IGetAllWpDtoQuery getAllWpDtoQuery,
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
            ICreateMapBomItemToRouteChartCommand createMapBomItemToRouteChartCommand)
        {
            var instanse = new ConsolidateBomItemsViewModel();
            await instanse.InitializeAsync(
                bomHeader,
                selectedBomItems,
                routeMapFactory,
                getAllProductDtoQuery,
                getAllWpDtoQuery,
                getAllMapsBomItemToRouteChartsQuery,
                createMapBomItemToRouteChartCommand);
            return instanse;
        }

        private bool _isReadOnlyComponent;
        public bool IsReadOnlyComponent
        {
            get
            {
                return _isReadOnlyComponent;
            }
            set
            {
                _isReadOnlyComponent = value;
                NotifyPropertyChanged(nameof(IsReadOnlyComponent));
            }
        }

        private bool _isEnabledComponent;
        public bool IsEnabledComponent
        {
            get
            {
                return _isEnabledComponent;
            }
            set
            {
                _isEnabledComponent = value;
                NotifyPropertyChanged(nameof(IsEnabledComponent));
            }
        }

        private bool _isConsolidateBomItemsByName;
        public bool IsConsolidateBomItemsByName
        {
            get
            {
                return _isConsolidateBomItemsByName;
            }
            set
            {
                _isConsolidateBomItemsByName = value;
                NotifyPropertyChanged(nameof(IsConsolidateBomItemsByName));
                LoadConsolidateBomItemsCommand?.Execute();
            }
        }

        private bool _isConsolidateBomItemsByNameEnabled;
        public bool IsConsolidateBomItemsByNameEnabled
        {
            get
            {
                return _isConsolidateBomItemsByNameEnabled;
            }
            set
            {
                _isConsolidateBomItemsByNameEnabled = value;
                NotifyPropertyChanged(nameof(IsConsolidateBomItemsByNameEnabled));
            }
        }

        public async Task RefreshQtyLaunched()
        {
            var links = await _getAllMapsBomItemToRouteChartsQuery.Execute();
            foreach (var row in Rows)
            {
                decimal sum = 0;

                foreach (var bi in row.BomItems)
                {
                    sum += links.Where(mk => mk.BomItemId == bi.Id).Sum(mk => mk.QtyLaunched);
                }

                row.QtyLaunched = (decimal)sum;
            }
        }
    }
}