using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    public class SelectedBomItemStore
    {
        private readonly IGetAllMapsBomItemToRouteChartsQuery _getAllMapsBomItemToRouteChartsQuery;
        private readonly IGetAllBomItemLogsQuery _getAllBomItemLogsQuery;
        private BomItem _selectedBomItem;

        public BomItem SelectedBomItem
        {
            get { return _selectedBomItem; }
            set
            {
                _selectedBomItem = value;
                SelectedBomItemChanged?.Invoke();
            }
        }

        public event Action SelectedBomItemChanged;

        public ObservableCollection<BomItemLog> BomItemLogs { get; }
        public ObservableCollection<MapBomItemToRouteChart> MapBomItemToRouteCharts { get; }

        public SelectedBomItemStore(
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
            IGetAllBomItemLogsQuery getAllBomItemLogsQuery)
        {
            _getAllMapsBomItemToRouteChartsQuery = getAllMapsBomItemToRouteChartsQuery;
            _getAllBomItemLogsQuery = getAllBomItemLogsQuery;

            BomItemLogs = new ObservableCollection<BomItemLog>();
            MapBomItemToRouteCharts = new ObservableCollection<MapBomItemToRouteChart>();

            SelectedBomItemChanged += OnSelectedBomItemChanged;
        }

        private async void OnSelectedBomItemChanged()
        {
            if (SelectedBomItem == null)
            {
                MapBomItemToRouteCharts.Clear();
                BomItemLogs.Clear();
                return;
            }

            var getAllMapsBomItemToRouteChartsQueryTask = _getAllMapsBomItemToRouteChartsQuery.ExecuteByBomItemId(SelectedBomItem.Id);
            var getAllBomItemLogsQueryTask = _getAllBomItemLogsQuery.ExecuteByBomItemId(SelectedBomItem.Id);

            await Task.WhenAll(getAllMapsBomItemToRouteChartsQueryTask, getAllBomItemLogsQueryTask);

            MapBomItemToRouteCharts.Clear();
            foreach (var mapBomItemToRouteChart in getAllMapsBomItemToRouteChartsQueryTask.Result)
                MapBomItemToRouteCharts.Add(mapBomItemToRouteChart);

            BomItemLogs.Clear();
            foreach (var bomItemLog in getAllBomItemLogsQueryTask.Result)
                BomItemLogs.Add(bomItemLog);
        }
    }
}