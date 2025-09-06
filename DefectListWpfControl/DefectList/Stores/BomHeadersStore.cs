using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    public class BomHeadersStore
    {
        private readonly IGetAllBomHeadersQuery _getAllBomHeadersQuery;
        private readonly IGetAllBomItemsWithSerialNumbersQuery _getAllBomItemsWithSerialNumbers;
        private readonly IGetAllRouteChartsTrackInfoQuery _getAllRouteChartsTrackInfoQuery;
        private readonly IGetAllBomItemsFilterToReportQuery _getAllBomItemsFilterToReportQuery;
        private readonly ICreateBomHeaderCommand _createBomHeaderCommand;
        private readonly IUpdateBomHeaderCommand _updateBomHeaderCommand;
        private readonly IDeleteBomHeaderCommand _deleteBomHeaderCommand;
        private readonly IUpdateStateBomHeaderCommand _updateStateBomHeaderCommand;
        private readonly ILoadBomItemsToBomHeaderCommand _loadBomItemsToBomHeaderCommand;
        private readonly List<BomHeader> _bomHeaders;
        private readonly Dictionary<string, IEnumerable<int>> _bomItemsWithSerialNumbers;

        public IEnumerable<BomHeader> BomHeaders => _bomHeaders;
        public Dictionary<string, IEnumerable<int>> BomItemsWithSerialNumbers => _bomItemsWithSerialNumbers;

        public BomHeadersStore(
            IGetAllBomHeadersQuery getAllBomHeadersQuery,
            IGetAllBomItemsWithSerialNumbersQuery getAllBomItemsWithSerialNumbers,
            IGetAllRouteChartsTrackInfoQuery getAllRouteChartsTrackInfoQuery,
            IGetAllBomItemsFilterToReportQuery getAllBomItemsFilterToReportQuery,
            ICreateBomHeaderCommand createBomHeaderCommand,
            IUpdateBomHeaderCommand updateBomHeaderCommand,
            IDeleteBomHeaderCommand deleteBomHeaderCommand,
            IUpdateStateBomHeaderCommand updateStateBomHeaderCommand,
            ILoadBomItemsToBomHeaderCommand loadBomItemsToBomHeaderCommand)
        {
            _getAllBomHeadersQuery = getAllBomHeadersQuery;
            _getAllBomItemsWithSerialNumbers = getAllBomItemsWithSerialNumbers;
            _getAllRouteChartsTrackInfoQuery = getAllRouteChartsTrackInfoQuery;
            _getAllBomItemsFilterToReportQuery = getAllBomItemsFilterToReportQuery;
            _createBomHeaderCommand = createBomHeaderCommand;
            _updateBomHeaderCommand = updateBomHeaderCommand;
            _deleteBomHeaderCommand = deleteBomHeaderCommand;
            _updateStateBomHeaderCommand = updateStateBomHeaderCommand;
            _loadBomItemsToBomHeaderCommand = loadBomItemsToBomHeaderCommand;
            _bomHeaders = new List<BomHeader>();
            _bomItemsWithSerialNumbers = new Dictionary<string, IEnumerable<int>>();
        }

        public event Action BomHeadersLoaded;
        public event Action<BomHeader> BomHeaderAdded;
        public event Action<BomHeader> BomHeaderUpdated;
        public event Action<int> BomHeaderDeleted;
        public event Action<BomHeader> BomHeaderStateUpdated;
        public event Action<BomHeader> BomHeaderLoadedById;
        public event Action<BomHeader> BomItemsToBomHeaderLoaded;

        public async Task Load()
        {
            var bomHeaders = await _getAllBomHeadersQuery.Execute();
            var bomItemsWithSerialNumbers = await _getAllBomItemsWithSerialNumbers.Execute();

            _bomHeaders.Clear();
            _bomHeaders.AddRange(bomHeaders);

            _bomItemsWithSerialNumbers.Clear();
            foreach (var b in bomItemsWithSerialNumbers)
                _bomItemsWithSerialNumbers.Add(b.Key, b.Value);

            BomHeadersLoaded?.Invoke();
        }

        public async Task<BomHeader> Load(int id)
        {
            var bomHeader = await _getAllBomHeadersQuery.Execute(id);

            int currentIndex = _bomHeaders.FindIndex(x => x.BomId == bomHeader.BomId);
            if (currentIndex != -1)
            {
                _bomHeaders[currentIndex] = bomHeader;
            }
            else
            {
                _bomHeaders.Add(bomHeader);
            }

            BomHeaderLoadedById?.Invoke(bomHeader);
            return bomHeader;
        }

        public async Task Add(BomHeader bomHeader)
        {
            await _createBomHeaderCommand.Execute(bomHeader);

            _bomHeaders.Add(bomHeader);

            BomHeaderAdded?.Invoke(bomHeader);
        }

        public async Task Update(BomHeader bomHeader)
        {
            await _updateBomHeaderCommand.Execute(bomHeader);

            int currentIndex = _bomHeaders.FindIndex(x => x.BomId == bomHeader.BomId);
            if (currentIndex != -1)
            {
                _bomHeaders[currentIndex] = bomHeader;
            }
            else
            {
                _bomHeaders.Add(bomHeader);
            }

            BomHeaderUpdated?.Invoke(bomHeader);
        }

        public async Task Delete(int id)
        {
            await _deleteBomHeaderCommand.Execute(id);

            _bomHeaders.RemoveAll(x => x.BomId == id);

            BomHeaderDeleted?.Invoke(id);
        }

        public async Task UpdateState(BomHeader bomHeader)
        {
            await _updateStateBomHeaderCommand.Execute(bomHeader);

            int currentIndex = _bomHeaders.FindIndex(x => x.BomId == bomHeader.BomId);
            if (currentIndex != -1)
            {
                _bomHeaders[currentIndex] = bomHeader;
            }
            else
            {
                _bomHeaders.Add(bomHeader);
            }

            BomHeaderStateUpdated?.Invoke(bomHeader);
        }

        public async Task LoadBomItemsToBomHeader(BomHeader bomHeader, string login)
        {
            await _loadBomItemsToBomHeaderCommand.Execute(bomHeader, login);

            BomItemsToBomHeaderLoaded?.Invoke(bomHeader);
        }

        public async Task<IEnumerable<RouteChartTrackInfo>> GetRouteChartsTrackInfos(string order)
        {
            var data = await _getAllRouteChartsTrackInfoQuery.Execute(order);
            return data;
        }

        public async Task<IEnumerable<string>> GetAllBomItemsFilterToReport(int rootItemId, string reportName)
        {
            return await _getAllBomItemsFilterToReportQuery.Execute(rootItemId, reportName);
        }
    }
}