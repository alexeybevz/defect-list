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
using DefectListWpfControl.ViewModelImplement;
using DefectListWpfControl.DefectList.Stores;

namespace DefectListWpfControl.DefectList.ViewModels
{
    public class ConsolidateBomItemsViewModel : ViewModel
    {
        private const int TargetWpId = 54; // 87-00  Управление цеха
        private const int PurchaseTargetWpId = 136; // 12-ПИ  Склад покупных комплектующих изделий

        private IGetAllMapsBomItemToRouteChartsQuery _getAllMapsBomItemToRouteChartsQuery;

        public ObservableCollection<ConsolidateBomItemViewModel> Rows { get; private set; }
        public IBomHeader BomHeader { get; private set; }
        public Dictionary<int, string> Depts { get; private set; }

        public ICommand CreateRouteChartsCommand { get; private set; }

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

            var bomItemViewModels = selectedBomItems.ToList();
            bomItemViewModels = bomItemViewModels
                .Where(x => (BomItemsConstantsStore.DecisionsToCreateRouteMap.Contains(x.Decision) || string.IsNullOrEmpty(x.Decision)) && !(x.Decision == "использовать" && x.DetalTyp == "матер"))
                .ToList();

            var rows = (await GetRows(bomItemViewModels, getAllMapsBomItemToRouteChartsQuery, getAllProductDtoQuery, Depts)).ToList();
            Rows = new ObservableCollection<ConsolidateBomItemViewModel>(rows);

            if (!rows.Any())
                throw new Exception("Для запуска МК нет строк, соответствующих условиям");

            CreateRouteChartsCommand = new CreateRouteChartsCommand(this, routeMapFactory, createMapBomItemToRouteChartCommand);
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

        private async Task<IEnumerable<ConsolidateBomItemViewModel>> GetRows(
            IEnumerable<BomItemViewModel> selectedBomItems,
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
            IGetAllProductDtoQuery getAllProductDtoQuery,
            Dictionary<int, string> depts)
        {
            var routeCharts = (await getAllMapsBomItemToRouteChartsQuery.Execute()).ToList();

            var validator = new Validator(new List<IValidationRule>()
            {
                new OtkRule(),
                new PdoRule()
            });

            var bomItemViewModels = selectedBomItems.ToList();
            var validatedBomItems = new List<ValidationResult>();
            foreach (var bi in bomItemViewModels)
            {
                var result = validator.Validate(new InputData()
                {
                    BomItem = bi.BomItem,
                    RouteCharts = routeCharts.Where(mk => mk.BomItemId == bi.Id).ToList()
                });

                if (result != null)
                    validatedBomItems.Add(result);
            }

            return validatedBomItems
                .GroupBy(x => new { x.BomItem.Detal, x.BomItem.DetalIma, x.BomItem.DetalTyp, x.TargetDetal, x.TargetWpId, x.NormalizeDecision, x.IsWorkNeed, x.IsPrint })
                .Select(x =>
                    new ConsolidateBomItemViewModel(getAllProductDtoQuery, depts)
                    {
                        BomItems = x.Select(y => y.BomItem).ToList(),
                        Detal = x.Key.Detal,
                        DetalIma = x.Key.DetalIma,
                        DetalTyp = x.Key.DetalTyp,
                        TargetDetal = x.Key.TargetDetal,
                        TargetWpId = x.Key.TargetWpId,
                        NormalizeDecision = x.Key.NormalizeDecision,
                        IsWorkNeed = x.Key.IsWorkNeed,
                        IsPrint = x.Key.IsPrint,
                        QtyMnf = (decimal)x.Sum(y => y.BomItem.QtyMnf),
                        QtyLaunched = (decimal)x.SelectMany(z => z.RouteCharts).Sum(mk => mk.QtyLaunched),
                    }
                )
                .ToList();
        }

        public async Task RefreshQtyLaunched()
        {
            var links = await _getAllMapsBomItemToRouteChartsQuery.Execute();
            foreach (var row in Rows)
            {
                float sum = 0;

                foreach (var bi in row.BomItems)
                {
                    sum += links.Where(mk => mk.BomItemId == bi.Id).Sum(mk => mk.QtyLaunched);
                }

                row.QtyLaunched = (decimal)sum;
            }
        }

        private class InputData
        {
            public IBomItem BomItem { get; set; }
            public List<MapBomItemToRouteChart> RouteCharts { get; set; }
        }

        private class ValidationResult
        {
            public IBomItem BomItem { get; set; }
            public string TargetDetal { get; set; }
            public int TargetWpId { get; set; }
            public bool IsPrint { get; set; }
            public bool IsWorkNeed { get; set; }
            public string NormalizeDecision { get; set; }
            public List<MapBomItemToRouteChart> RouteCharts { get; set; }
        }

        private interface IValidationRule
        {
            bool IsMatch(InputData vm);
            ValidationResult GetResult(InputData vm);
        }

        private class OtkRule : IValidationRule
        {
            public bool IsMatch(InputData vm)
            {
                var allowedDecisions = new[] { "ремонт", "использовать" };
                return (PermissionsStore.IsCanOtkCreateRouteMapsUser || PermissionsStore.IsCanPdoCreateRouteMapsUser) && allowedDecisions.Contains(vm.BomItem.Decision);
            }

            public ValidationResult GetResult(InputData vm)
            {
                return new ValidationResult()
                {
                    BomItem = vm.BomItem,
                    TargetDetal = vm.BomItem.Detals.EndsWith("В") || vm.BomItem.Detals.EndsWith("В1") ? vm.BomItem.Detal : vm.BomItem.Detal + "Р",
                    TargetWpId = TargetWpId,
                    NormalizeDecision = vm.BomItem.Decision,
                    IsPrint = true,
                    IsWorkNeed = GetIsWorkNeeded(vm.BomItem),
                    RouteCharts = vm.RouteCharts
                };
            }

            private bool GetIsWorkNeeded(IBomItem vm)
            {
                var denyRules = new List<bool>
                {
                    vm.Defect == "соответствует КД" && vm.FinalDecision == "использовать",
                    vm.Defect == "соответствует КД" && vm.Decision == "использовать"
                };

                // если хотя бы одно правило истинно, то нужен запрет (false) на выполнение работ по создаваемым МК
                return !denyRules.Any(x => x);
            }
        }

        private class PdoRule : IValidationRule
        {
            public bool IsMatch(InputData vm)
            {
                var allowedDecisions = new[] { "заменить", "скомплектовать", null, "" };
                return PermissionsStore.IsCanPdoCreateRouteMapsUser && allowedDecisions.Contains(vm.BomItem.FinalDecision);
            }

            public ValidationResult GetResult(InputData vm)
            {
                return new ValidationResult()
                {
                    BomItem = vm.BomItem,
                    TargetDetal = vm.BomItem.Detal,
                    TargetWpId = vm.BomItem.IsPki || vm.BomItem.IsMaterial ? PurchaseTargetWpId : TargetWpId,
                    NormalizeDecision = NormalizeSolution(vm.BomItem.FinalDecision),
                    IsWorkNeed = !vm.BomItem.IsPki && !vm.BomItem.IsMaterial,
                    IsPrint = true,
                    RouteCharts = vm.RouteCharts
                };
            }

            private string NormalizeSolution(string decision)
            {
                if (decision == "заменить" || decision == "скомплектовать")
                    return "заменить/скомплектовать";
                return decision;
            }
        }

        private class Validator
        {
            private readonly List<IValidationRule> _rules;

            public Validator(IEnumerable<IValidationRule> rules)
            {
                _rules = rules.ToList();
            }

            public ValidationResult Validate(InputData vm)
            {
                foreach (var rule in _rules)
                {
                    if (rule.IsMatch(vm))
                        return rule.GetResult(vm);
                }

                return null;
            }
        }
    }
}