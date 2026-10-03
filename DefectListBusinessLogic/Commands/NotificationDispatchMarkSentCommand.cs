using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class NotificationDispatchMarkSentCommand : DbConnectionPmControlRepositoryBase, INotificationDispatchMarkSentCommand
    {
        public NotificationDispatchMarkSentCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(string hash) =>
            await DbConnection.ExecuteAsync(Query, new { hash });

        private const string Query = @"
            UPDATE NotificationDispatches
            SET
                  Status = 'Sent'
                , SentAt = getdate()
            WHERE PayloadHash = @hash";
    }
}