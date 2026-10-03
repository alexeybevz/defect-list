using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Queries
{
    public class GetAllNotificationEventSubscribersQuery : DbConnectionPmControlRepositoryBase, IGetAllNotificationEventSubscribersQuery
    {
        public GetAllNotificationEventSubscribersQuery(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<NotificationEventSubscriber>> Execute() =>
            (await DbConnection.QueryAsync<NotificationEventSubscriber>(Query)).ToList();

        public async Task<NotificationEventSubscriber> Execute(int userId, NotificationEventType notificationEventType) =>
            await DbConnection.QuerySingleOrDefaultAsync<NotificationEventSubscriber>(
                Query + " WHERE nes.UserId = @UserId AND net.Id = @NotificationEventType",
                new { UserId = userId, NotificationEventType = notificationEventType});

        private const string Query = @"
            SELECT
                  nes.Id
                , nes.UserId
                , nes.IsEnabled
                , nes.CreatedAt
                , net.Id AS NotificationEventType
            FROM NotificationEventSubscribers nes
            INNER JOIN NotificationEventType net ON net.Id = nes.NotificationEventTypeId";
    }
}