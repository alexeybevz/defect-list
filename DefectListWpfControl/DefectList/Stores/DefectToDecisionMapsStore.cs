using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListWpfControl.DefectList.Stores
{
    public class DefectToDecisionMapsStore
    {
        private readonly IGetAllDefectToDecisionMapsQuery _getAllDefectToDecisionMapsQuery;
        private readonly IGetAllStateDetalsQuery _getAllStateDetalsQuery;
        private readonly IGetAllGroupDefectsQuery _getAllGroupDefectsQuery;
        private readonly IGetAllDecisionsQuery _getAllDecisionsQuery;
        private readonly ICreateDefectToDecisionMapCommand _createDefectToDecisionMapCommand;
        private readonly IUpdateDefectToDecisionMapCommand _updateDefectToDecisionMapCommand;
        private readonly IDeleteDefectToDecisionMapCommand _deleteDefectToDecisionMapCommand;

        private readonly List<MapDefectToDecision> _defectToDecisionMaps;
        public IEnumerable<MapDefectToDecision> DefectToDecisionMaps => _defectToDecisionMaps;

        private readonly List<StateDetals> _detalStates;
        public IEnumerable<StateDetals> DetalStates => _detalStates;

        private readonly List<GroupDefect> _defectGroups;
        public IEnumerable<GroupDefect> DefectGroups => _defectGroups;

        private readonly List<string> _decisions;
        public IEnumerable<string> Decision => _decisions;

        public event Action DefectToDecisionMapsLoaded;
        public event Action<MapDefectToDecision> DefectToDecisionMapAdded;
        public event Action<MapDefectToDecision> DefectToDecisionMapUpdated;
        public event Action<int> DefectToDecisionMapDeleted;

        public DefectToDecisionMapsStore(
            IGetAllDefectToDecisionMapsQuery getAllDefectToDecisionMapsQuery,
            IGetAllStateDetalsQuery getAllStateDetalsQuery,
            IGetAllGroupDefectsQuery getAllGroupDefectsQuery,
            IGetAllDecisionsQuery getAllDecisionsQuery,
            ICreateDefectToDecisionMapCommand createDefectToDecisionMapCommand,
            IUpdateDefectToDecisionMapCommand updateDefectToDecisionMapCommand,
            IDeleteDefectToDecisionMapCommand deleteDefectToDecisionMapCommand)
        {
            _getAllDefectToDecisionMapsQuery = getAllDefectToDecisionMapsQuery;
            _getAllStateDetalsQuery = getAllStateDetalsQuery;
            _getAllGroupDefectsQuery = getAllGroupDefectsQuery;
            _getAllDecisionsQuery = getAllDecisionsQuery;
            _createDefectToDecisionMapCommand = createDefectToDecisionMapCommand;
            _updateDefectToDecisionMapCommand = updateDefectToDecisionMapCommand;
            _deleteDefectToDecisionMapCommand = deleteDefectToDecisionMapCommand;

            _defectToDecisionMaps = new List<MapDefectToDecision>();
            _detalStates = new List<StateDetals>();
            _defectGroups = new List<GroupDefect>();
            _decisions = new List<string>();
        }

        public async Task Load()
        {
            var getAllDefectToDecisionMapsQueryTask = _getAllDefectToDecisionMapsQuery.Execute();
            var getAllStateDetalsQueryTask = _getAllStateDetalsQuery.Execute();
            var getAllGroupDefectsQueryTask = _getAllGroupDefectsQuery.Execute();
            var getAllDecisionsQueryTask = _getAllDecisionsQuery.Execute();

            await Task.WhenAll(
                getAllDefectToDecisionMapsQueryTask,
                getAllStateDetalsQueryTask,
                getAllGroupDefectsQueryTask,
                getAllDecisionsQueryTask);

            _defectToDecisionMaps.Clear();
            _detalStates.Clear();
            _defectGroups.Clear();
            _decisions.Clear();

            _defectToDecisionMaps.AddRange(getAllDefectToDecisionMapsQueryTask.Result.ToList());
            _detalStates.AddRange(getAllStateDetalsQueryTask.Result);
            _defectGroups.AddRange(getAllGroupDefectsQueryTask.Result);
            _decisions.AddRange(getAllDecisionsQueryTask.Result);

            DefectToDecisionMapsLoaded?.Invoke();
        }

        public async Task Add(MapDefectToDecision mapDefectToDecision)
        {
            await _createDefectToDecisionMapCommand.Execute(mapDefectToDecision);

            _defectToDecisionMaps.Add(mapDefectToDecision);

            DefectToDecisionMapAdded?.Invoke(mapDefectToDecision);
        }

        public async Task Update(MapDefectToDecision mapDefectToDecision)
        {
            await _updateDefectToDecisionMapCommand.Execute(mapDefectToDecision);

            int currentIndex = _defectToDecisionMaps.FindIndex(x => x.Id == mapDefectToDecision.Id);
            if (currentIndex != -1)
            {
                _defectToDecisionMaps[currentIndex] = mapDefectToDecision;
            }
            else
            {
                _defectToDecisionMaps.Add(mapDefectToDecision);
            }

            DefectToDecisionMapUpdated?.Invoke(mapDefectToDecision);
        }

        public async Task Delete(int id)
        {
            await _deleteDefectToDecisionMapCommand.Execute(id);

            _defectToDecisionMaps.RemoveAll(x => x.Id == id);

            DefectToDecisionMapDeleted?.Invoke(id);
        }
    }
}