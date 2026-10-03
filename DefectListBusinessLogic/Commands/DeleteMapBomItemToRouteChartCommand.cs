using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class DeleteMapBomItemToRouteChartCommand : DbConnectionPmControlRepositoryBase, IDeleteMapBomItemToRouteChartCommand
    {
        private const string Query = @"DELETE FROM MapBomItemToRouteChart WHERE RouteChart_Number = @routeChartNumber";

        public DeleteMapBomItemToRouteChartCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(string routeChartNumber) =>
            await DbConnection.ExecuteAsync(Query, new { routeChartNumber });
    }
}