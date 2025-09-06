using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class UnSubscribeUserOnBomHeaderCommand : DbConnectionPmControlRepositoryBase, IUnSubscribeUserOnBomHeaderCommand
    {
        public UnSubscribeUserOnBomHeaderCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(BomHeaderSubscriber bomHeaderSubscriber)
        {
            var query = "DELETE FROM BomHeaderSubscribers WHERE UserId = @UserId AND BomId = @BomId;";
            using (var db = await CreateOpenConnectionAsync())
            {
                await db.ExecuteAsync(query, new { bomHeaderSubscriber.UserId, bomHeaderSubscriber.BomId });
            }
        }
    }
}