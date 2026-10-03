using System.Threading.Tasks;
using DefectListDomain.Commands;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using Dapper;

namespace DefectListBusinessLogic.Commands
{
    public class NotificationDispatchMarkFailedCommand : DbConnectionPmControlRepositoryBase, INotificationDispatchMarkFailedCommand
    {
        public NotificationDispatchMarkFailedCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(string hash, string error) =>
            await DbConnection.ExecuteAsync(Query, new { hash, error });

        private const string Query = @"
            UPDATE NotificationDispatches
            SET
                  Status = 'Failed'
                , Attempts = Attempts + 1
                , LastAttempAt = getdate()
                , Error = @error
            WHERE PayloadHash = @hash";
    }
}