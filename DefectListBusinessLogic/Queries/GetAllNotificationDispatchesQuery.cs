using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllNotificationDispatchesQuery : DbConnectionPmControlRepositoryBase, IGetAllNotificationDispatchesQuery
    {
        public GetAllNotificationDispatchesQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<NotificationDispatch> Execute(string hash) =>
            await DbConnection.QuerySingleOrDefaultAsync<NotificationDispatch>(Query, new { hash });

        private const string Query = @"
            SELECT
                  Id
                , UserId
                , NotificationEventTypeId AS NotificationEventType
                , PayloadHash
                , Status
                , Attempts
                , ScheduleAt
                , SentAt
                , LastAttempAt
                , Error
            FROM NotificationDispatches
            WHERE PayloadHash = @hash";
    }
}