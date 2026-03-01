using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Queries;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using System.Linq;
using System.Windows;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using ReporterBusinessLogic;

namespace DefectListWpfControl.DefectList.Commands.ConsolidateBomItemsCommands
{
    public class LoadConsolidateBomItemsCommand : AsyncCommandBase, IDisposable
    {
        private const int TargetWpId = 54; // 87-00  Управление цеха
        private const int PurchaseTargetWpId = 136; // 12-ПИ  Склад покупных комплектующих изделий

        private readonly ConsolidateBomItemsViewModel _consolidateBomItemsViewModel;
        private readonly IEnumerable<BomItemViewModel> _selectedBomItems;
        private readonly IGetAllMapsBomItemToRouteChartsQuery _getAllMapsBomItemToRouteChartsQuery;
        private readonly IGetAllProductDtoQuery _getAllProductDtoQuery;

        public LoadConsolidateBomItemsCommand(
            ConsolidateBomItemsViewModel consolidateBomItemsViewModel,
            IEnumerable<BomItemViewModel> selectedBomItems,
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
            IGetAllProductDtoQuery getAllProductDtoQuery)
        {
            _consolidateBomItemsViewModel = consolidateBomItemsViewModel;
            _selectedBomItems = selectedBomItems;
            _getAllMapsBomItemToRouteChartsQuery = getAllMapsBomItemToRouteChartsQuery;
            _getAllProductDtoQuery = getAllProductDtoQuery;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                _consolidateBomItemsViewModel.IsConsolidateBomItemsByNameEnabled = false;

                foreach (var vm in _consolidateBomItemsViewModel.Rows)
                {
                    vm.TargetDetalUpdated -= TargetDetalUpdatedAction;
                    vm.TargetWpIdUpdated -= TargetWpIdUpdatedAction;
                }

                _consolidateBomItemsViewModel.Rows.Clear();

                var bomItemViewModels = _selectedBomItems.ToList();
                bomItemViewModels = bomItemViewModels
                    .Where(x =>
                        (BomItemsConstantsStore.DecisionsToCreateRouteMap.Contains(x.Decision) ||
                         string.IsNullOrEmpty(x.Decision)) && !(x.Decision == "использовать" && x.DetalTyp == "матер"))
                    .ToList();

                var rows = (await GetRows(bomItemViewModels, _getAllMapsBomItemToRouteChartsQuery, _consolidateBomItemsViewModel.Depts)).ToList();
                foreach (var vm in rows)
                {
                    vm.TargetDetalUpdated += TargetDetalUpdatedAction;
                    vm.TargetWpIdUpdated += TargetWpIdUpdatedAction;

                    // при создании VM установлено значение TargetDetal, но для инициализации остальных значений в первый раз метод события нужно выполнить вручную.
                    await TargetDetalUpdatedAction(vm, vm.TargetDetal);

                    TargetWpIdUpdatedAction(vm, vm.TargetWpId);

                    _consolidateBomItemsViewModel.Rows.Add(vm);
                }
            }
            finally
            {
                _consolidateBomItemsViewModel.IsConsolidateBomItemsByNameEnabled = true;
            }
        }

        private void TargetWpIdUpdatedAction(ConsolidateBomItemViewModel vm, int targetWpId)
        {
            vm.TargetWp = _consolidateBomItemsViewModel.Depts.FirstOrDefault(x => x.Key == targetWpId);
            vm.TargetWpName = _consolidateBomItemsViewModel.Depts.FirstOrDefault(x => x.Key == targetWpId).Value;
        }

        private async Task TargetDetalUpdatedAction(ConsolidateBomItemViewModel vm, string detal)
        {
            try
            {
                vm.ProductId = (await _getAllProductDtoQuery.ExecuteByDetals(SpecifKeyCreator.CreateKey(detal)))?.Id ?? 0;
                vm.TargetDetals = SpecifKeyCreator.CreateKey(detal);
                vm.IsSelected = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Произошла ошибка при обращении к базе данных во время работы с полем 'Обозначение для запуска МК в АСУП'");
            }
        }

        public void Dispose()
        {
            foreach (var vm in _consolidateBomItemsViewModel.Rows)
            {
                vm.TargetDetalUpdated -= TargetDetalUpdatedAction;
                vm.TargetWpIdUpdated -= TargetWpIdUpdatedAction;
            }
        }

        private async Task<IEnumerable<ConsolidateBomItemViewModel>> GetRows(
            IEnumerable<BomItemViewModel> selectedBomItems,
            IGetAllMapsBomItemToRouteChartsQuery getAllMapsBomItemToRouteChartsQuery,
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

            if (_consolidateBomItemsViewModel.IsConsolidateBomItemsByName)
                return validatedBomItems
                    .GroupBy(x => new { x.BomItem.Detal, x.BomItem.DetalIma, x.BomItem.DetalTyp, x.TargetDetal, x.TargetWpId, x.NormalizeDecision, x.IsWorkNeed, x.IsPrint })
                    .Select(x =>
                        new ConsolidateBomItemViewModel(depts)
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

            return validatedBomItems
                .GroupBy(x => new { x.BomItem.Id, x.BomItem.Detal, x.BomItem.DetalIma, x.BomItem.DetalTyp, x.TargetDetal, x.TargetWpId, x.NormalizeDecision, x.IsWorkNeed, x.IsPrint })
                .Select(x =>
                    new ConsolidateBomItemViewModel(depts)
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