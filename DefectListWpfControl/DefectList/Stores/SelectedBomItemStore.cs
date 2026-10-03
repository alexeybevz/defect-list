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
        private readonly IGetAllMeasurementMapItemLogsQuery _getAllMeasurementMapItemLogsQuery;
        private readonly IGetMeasurementMapDictionaryByCodeQuery _getMeasurementMapDictionaryByCodeQuery;
        private BomItem _selectedBomItem;

        public BomItem SelectedBomItem
        {
            get { return _selectedBomItem; }
            set
            {
                _selectedBomItem = value;
            }
        }

        public ObservableCollection<BomItemLog> BomItemLogs { get; }
        public ObservableCollection<MapBomItemToRouteChart> MapBomItemToRouteCharts { get; }
        public ObservableCollection<MeasurementMapItemLog> MeasurementMapItemLogs { get; }
        public MeasurementMapDictionary MeasurementMapDictionary { get; private set; }

        public SelectedBomItemStore(
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
            IGetAllBomItemLogsQuery getAllBomItemLogsQuery,
            IGetAllMeasurementMapItemLogsQuery getAllMeasurementMapItemLogsQuery,
            IGetMeasurementMapDictionaryByCodeQuery getMeasurementMapDictionaryByCodeQuery)
        {
            _getAllMapsBomItemToRouteChartsQuery = getAllMapsBomItemToRouteChartsQuery;
            _getAllBomItemLogsQuery = getAllBomItemLogsQuery;
            _getAllMeasurementMapItemLogsQuery = getAllMeasurementMapItemLogsQuery;
            _getMeasurementMapDictionaryByCodeQuery = getMeasurementMapDictionaryByCodeQuery;

            BomItemLogs = new ObservableCollection<BomItemLog>();
            MapBomItemToRouteCharts = new ObservableCollection<MapBomItemToRouteChart>();
            MeasurementMapItemLogs = new ObservableCollection<MeasurementMapItemLog>();
        }

        public async Task LoadDataOnSelectedBomItemChanged()
        {
            if (SelectedBomItem == null)
            {
                MapBomItemToRouteCharts.Clear();
                BomItemLogs.Clear();
                MeasurementMapItemLogs.Clear();
                return;
            }

            var getAllMapsBomItemToRouteChartsQueryTask = _getAllMapsBomItemToRouteChartsQuery.ExecuteByBomItemId(SelectedBomItem.Id);
            var getAllBomItemLogsQueryTask = _getAllBomItemLogsQuery.ExecuteByBomItemId(SelectedBomItem.Id);
            var getAllMeasurementMapItemLogsQueryTask = _getAllMeasurementMapItemLogsQuery.ExecuteByBomItemIdAsync(SelectedBomItem.Id);

            await Task.WhenAll(
                getAllMapsBomItemToRouteChartsQueryTask,
                getAllBomItemLogsQueryTask,
                getAllMeasurementMapItemLogsQueryTask);

            MapBomItemToRouteCharts.Clear();
            foreach (var mapBomItemToRouteChart in getAllMapsBomItemToRouteChartsQueryTask.Result)
                MapBomItemToRouteCharts.Add(mapBomItemToRouteChart);

            BomItemLogs.Clear();
            foreach (var bomItemLog in getAllBomItemLogsQueryTask.Result)
                BomItemLogs.Add(bomItemLog);

            MeasurementMapItemLogs.Clear();
            foreach (var measurementMapItemLog in getAllMeasurementMapItemLogsQueryTask.Result)
                MeasurementMapItemLogs.Add(measurementMapItemLog);
        }

        public async Task LoadMeasurementMapDictionaryOnSelectedBomItemChanged(int? codeLsf82, int rootItemId, bool isHasMeasurementMap)
        {
            if (codeLsf82.HasValue && !isHasMeasurementMap)
                MeasurementMapDictionary = await _getMeasurementMapDictionaryByCodeQuery.ExecuteAsync(codeLsf82.Value, rootItemId);
            else
                MeasurementMapDictionary = null;
        }
    }
}