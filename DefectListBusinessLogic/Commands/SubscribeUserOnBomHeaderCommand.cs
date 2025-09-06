using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;

namespace DefectListBusinessLogic.Commands
{
    public class SubscribeUserOnBomHeaderCommand : DbConnectionPmControlRepositoryBase, ISubscribeUserOnBomHeaderCommand
    {
        public SubscribeUserOnBomHeaderCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(BomHeaderSubscriber bomHeaderSubscriber)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                var parm = new { bomHeaderSubscriber.UserId, bomHeaderSubscriber.BomId };

                var isExists = await db.ExecuteScalarAsync<bool>("SELECT 1 FROM BomHeaderSubscribers WHERE UserId = @UserId AND BomId = @BomId", parm);

                if (isExists)
                    return;

                await db.ExecuteAsync("INSERT INTO BomHeaderSubscribers (UserId, BomId) VALUES (@UserId, @BomId);", parm);
            }
        }
    }
}