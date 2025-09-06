using System.Data;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;

namespace DefectListBusinessLogic.Commands
{
    public class UpdateBomItemIdInMapToRouteChartCommand : DbConnectionPmControlRepositoryBase, IUpdateBomItemIdInMapToRouteChartCommand
    {
        public UpdateBomItemIdInMapToRouteChartCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        private const string Query =
            @"UPDATE MapBomItemToRouteChart SET BomItemId = @newBomItemId WHERE BomItemId = @oldBomItemId";

        public Task Execute(int oldBomItemId, int newBomItemId) =>
            DbConnection.ExecuteAsync(Query, new {oldBomItemId, newBomItemId});

        public void Execute(int oldBomItemId, int newBomItemId, IDbConnection db, IDbTransaction transaction)
        {
            db.Execute(Query, new { oldBomItemId, newBomItemId }, transaction);
        }
    }
}