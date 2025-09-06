using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.Commands;
using DefectListDomain.Dtos;
using DefectListDomain.ExternalData;
using DefectListDomain.Models;
using DefectListWpfControl.DefectList.Stores;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.BomItemCommands
{
    public class CreateRouteChartsCommand : AsyncCommandBase
    {
        private readonly CustomIdentity _user;
        private readonly ConsolidateBomItemsViewModel _consolidateBomItemsViewModel;
        private readonly IRouteMapFactory _routeMapFactory;
        private readonly ICreateMapBomItemToRouteChartCommand _createMapBomItemToRouteChartCommand;

        public CreateRouteChartsCommand(
            ConsolidateBomItemsViewModel consolidateBomItemsViewModel,
            IRouteMapFactory routeMapFactory,
            ICreateMapBomItemToRouteChartCommand createMapBomItemToRouteChartCommand)
        {
            _consolidateBomItemsViewModel = consolidateBomItemsViewModel;
            _routeMapFactory = routeMapFactory;
            _createMapBomItemToRouteChartCommand = createMapBomItemToRouteChartCommand;
            _user = Thread.CurrentPrincipal.Identity as CustomIdentity;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (_consolidateBomItemsViewModel.BomHeader.CreateDateFirstRouteMap != null)
            {
                var endDateCreateRouteCharts = _consolidateBomItemsViewModel.BomHeader.CreateDateFirstRouteMap.Value.Date.AddDays(BomItemsConstantsStore.CountDaysAllowedCreateRouteCharts + 1);
                if (DateTime.Now > endDateCreateRouteCharts && !PermissionsStore.IsCanUnrestrictedCreateRouteMapsUser)
                {
                    MessageBox.Show(
                        $"Срок создания маршрутных карт истек.\n\nДата создания первой МК: {_consolidateBomItemsViewModel.BomHeader.CreateDateFirstRouteMap.Value.Date.ToShortDateString()}\n" +
                        $"Крайний срок: {endDateCreateRouteCharts.Date.AddDays(-1).ToShortDateString()} (включительно)", "Внимание",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var sessionGuid = Guid.NewGuid().ToString();
            var selectedRows = _consolidateBomItemsViewModel.Rows
                .Where(x => x.IsSelected)
                .ToList();

            if (!selectedRows.Any())
                return;

            var tasks = CreateTasks(sessionGuid, selectedRows);

            _routeMapFactory.AddTasksOnCreateRouteMaps(tasks);
            _routeMapFactory.RunMarKartaExeToHandleTasksOnCreate(sessionGuid);

            var handledTasks = _routeMapFactory.AskHandledTasks(sessionGuid);

            foreach (var x in handledTasks)
            {
                var obj = selectedRows.First(b => b.RowGuid == x.RowGuid);
                obj.Comment = x.Comment;
                obj.CreatedRouteMap = x.CreatedRouteMap;
                obj.CreatedRouteMapId = x.CreatedRouteMapId;

                if (x.Status != 1 || string.IsNullOrEmpty(x.CreatedRouteMap))
                    continue;

                if (obj.BomItems.Count() > 1)
                {
                    foreach (var bi in obj.BomItems)
                    {
                        await _createMapBomItemToRouteChartCommand.Execute(new MapBomItemToRouteChart()
                        {
                            BomItemId = bi.Id,
                            MkartaId = x.CreatedRouteMapId,
                            RouteChart_Number = x.CreatedRouteMap,
                            QtyLaunched = bi.QtyMnf,
                            ProductId = obj.ProductId,
                            Detal = obj.TargetDetal,
                            CreatedBy = _user.Name
                        });
                    }
                }
                else
                {
                    var bi = obj.BomItems.First();
                    await _createMapBomItemToRouteChartCommand.Execute(new MapBomItemToRouteChart()
                    {
                        BomItemId = bi.Id,
                        MkartaId = x.CreatedRouteMapId,
                        RouteChart_Number = x.CreatedRouteMap,
                        QtyLaunched = x.Qty,
                        ProductId = obj.ProductId,
                        Detal = obj.TargetDetal,
                        CreatedBy = _user.Name
                    });
                }

                obj.RowGuid = string.Empty;
            }

            await _consolidateBomItemsViewModel.RefreshQtyLaunched();
        }

        private List<RouteMapIntegrationTaskDto> CreateTasks(string sessionGuid, IEnumerable<ConsolidateBomItemViewModel> vms)
        {
            var result = new List<RouteMapIntegrationTaskDto>();

            foreach (var x in vms)
            {
                var rowGuid = Guid.NewGuid().ToString();
                x.RowGuid = rowGuid;

                result.Add(new RouteMapIntegrationTaskDto()
                {
                    Id = 0,
                    SessionGuid = sessionGuid,
                    RowGuid = rowGuid,
                    OrdersCode = _consolidateBomItemsViewModel.BomHeader.Orders,
                    OrdersYear = DateTime.Now.Year,
                    TargetWpId = x.TargetWpId,
                    BomItemId = x.BomItems.Count() == 1 ? x.BomItems.FirstOrDefault()?.Id ?? -1 : -1,
                    BomItemIds = string.Join(",", x.BomItems.Select(b => b.Id).ToList()),
                    ProductId = x.ProductId,
                    Detals = x.TargetDetals,
                    Detal = x.TargetDetal,
                    Qty = (float)x.QtyForLaunch,
                    PmcontrUserName = _user.CN,
                    Status = 0,
                    IsPrint = x.IsPrint,
                    IsWorkNeed = x.IsWorkNeed,
                    TypeProg = "DEFECT_LIST",
                });
            }

            return result;
        }
    }
}