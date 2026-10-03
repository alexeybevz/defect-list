using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DefectListDomain.CreatingReports;
using DefectListDomain.ExternalData;
using DefectListWpfControl.DefectList.ViewModels;
using DefectListWpfControl.ViewModelImplement;
using ReporterDomain.Auth;

namespace DefectListWpfControl.DefectList.Commands.ReportCommands
{
    public class ExportCreatedRouteMapsCommand : AsyncCommandBase
    {
        private readonly ConsolidateBomItemsViewModel _consolidateBomItemsViewModel;
        private readonly IGetAllRouteMapDtoQuery _getAllRouteMapDtoQuery;
        private readonly CustomIdentity _user;

        public ExportCreatedRouteMapsCommand(ConsolidateBomItemsViewModel consolidateBomItemsViewModel, IGetAllRouteMapDtoQuery getAllRouteMapDtoQuery)
        {
            _consolidateBomItemsViewModel = consolidateBomItemsViewModel;
            _getAllRouteMapDtoQuery = getAllRouteMapDtoQuery;
            _user = Thread.CurrentPrincipal.Identity as CustomIdentity;
        }

        public override async Task ExecuteAsync(object parameter = null)
        {
            try
            {
                var routeMaps = _consolidateBomItemsViewModel.Rows
                    .Where(x => x.CreatedRouteMapId > 0)
                    .Select(x => x.CreatedRouteMap)
                    .ToList();

                if (!routeMaps.Any())
                {
                    MessageBox.Show("Нет данных для формирования отчета");
                    return;
                }

                var routeMapDtos = (await _getAllRouteMapDtoQuery.ExecuteAsync(routeMaps)).ToList();

                if (!routeMapDtos.Any())
                {
                    MessageBox.Show("Нет данных для формирования отчета");
                    return;
                }

                var report = DefectListIocKernel.Get<IExportCreatedRouteMapsReport>();
                await report.CreateAsync(_consolidateBomItemsViewModel.BomHeader, routeMapDtos, _user.Name);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}