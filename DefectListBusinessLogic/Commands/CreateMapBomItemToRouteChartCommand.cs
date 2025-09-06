using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class CreateMapBomItemToRouteChartCommand : DbConnectionPmControlRepositoryBase, ICreateMapBomItemToRouteChartCommand
    {
        public CreateMapBomItemToRouteChartCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        private const string Query = @"
            INSERT INTO MapBomItemToRouteChart (BomItemId, MkartaId, RouteChart_Number, QtyLaunched, CreatedBy, ProductId, Detal)
            VALUES (@BomItemId, @MkartaId, @RouteChart_Number, @QtyLaunched, @CreatedBy, @ProductId, @Detal)";

        public async Task Execute(MapBomItemToRouteChart mapBomItemToRouteChart) =>
            await DbConnection.ExecuteAsync(Query, mapBomItemToRouteChart);
    }
}