using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListDomain.ExternalData;
using DefectListDomain.Dtos;

namespace DefectListWpfControl.DefectList.Stores
{
    public class TehprocHeadersStore
    {
        private readonly IGetAllTehprocHeaderDtoQuery _getAllTehprocHeadersQuery;

        private readonly Dictionary<string, TehprocHeaderDto> _tehprocHeaders;
        public IReadOnlyDictionary<string, TehprocHeaderDto> TehprocHeaders => _tehprocHeaders;

        public event Action TehprocHeadersLoaded;
        public event Action<TehprocHeaderDto> TehprocHeadersLoadedByDetals;

        public TehprocHeadersStore(IGetAllTehprocHeaderDtoQuery getAllTehprocHeadersQuery)
        {
            _getAllTehprocHeadersQuery = getAllTehprocHeadersQuery;

            _tehprocHeaders = new Dictionary<string, TehprocHeaderDto>();
        }

        public async Task Load()
        {
            var tehprocHeaders = await _getAllTehprocHeadersQuery.Execute();

            _tehprocHeaders.Clear();
            foreach (var tehprocHeader in tehprocHeaders)
            {
                _tehprocHeaders.Add(tehprocHeader.Detals, tehprocHeader);
            }

            TehprocHeadersLoaded?.Invoke();
        }

        public async Task LoadByDetals(string detals)
        {
            var tehprocHeader = await _getAllTehprocHeadersQuery.ExecuteByDetals(detals);

            if (_tehprocHeaders.ContainsKey(detals))
            {
                if (tehprocHeader == null)
                    _tehprocHeaders.Remove(detals);
                else
                    _tehprocHeaders[detals] = tehprocHeader;
            }

            TehprocHeadersLoadedByDetals?.Invoke(tehprocHeader);
        }

        public TechnologicalProcessResult GetSummaryInfo(BomItemViewModel bomItem)
        {
            if (bomItem == null)
                throw new ArgumentException("Для определения применяемого техпроцесса не передано ДСЕ");

            var tp1 = _tehprocHeaders.ContainsKey(bomItem.Detals);
            var tp2 = _tehprocHeaders.ContainsKey(bomItem.Detals + "Р");

            if (bomItem.Decision == "ремонт" || bomItem.FinalDecision == "ремонт")
            {
                if (tp1 && (bomItem.Detal.Last() == 'Р' && !bomItem.Detal.EndsWith("ОБР") ||
                            bomItem.Detal.Last() == 'В' && !bomItem.Detal.EndsWith("ОСВ")))
                {
                    return new TechnologicalProcessResult()
                    {
                        Value = bomItem.IsPki ? null : bomItem.Detal,
                        IsExistsTechnologicalProcess = true,
                        IsRepair = true,
                        IsReplace = false,
                    };
                }

                return new TechnologicalProcessResult()
                {
                    Value = bomItem.IsPki ? null : (tp2 ? bomItem.Detal + "Р" : null),
                    IsExistsTechnologicalProcess = tp1 || tp2,
                    IsRepair = tp2,
                    IsReplace = tp1,
                };
            }

            return new TechnologicalProcessResult()
            {
                Value = null,
                IsExistsTechnologicalProcess = tp1 || tp2,
                IsRepair = tp2,
                IsReplace = tp1,
            };
        }
    }

    public class TechnologicalProcessResult
    {
        public bool IsExistsTechnologicalProcess { get; set; }
        public string Value { get; set; }
        public bool IsRepair { get; set; }
        public bool IsReplace { get; set; }
    }
}