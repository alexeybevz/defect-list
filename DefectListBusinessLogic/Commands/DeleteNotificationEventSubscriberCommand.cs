using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class DeleteNotificationEventSubscriberCommand : DbConnectionPmControlRepositoryBase, IDeleteNotificationEventSubscriberCommand
    {
        public DeleteNotificationEventSubscriberCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task Execute(int userId, NotificationEventType notificationEventType) =>
            await DbConnection.ExecuteAsync(Query, new { UserId = userId, NotificationEventTypeId = notificationEventType });

        private const string Query = @"
            DELETE FROM NotificationEventSubscribers
            WHERE UserId = @UserId AND NotificationEventTypeId = @NotificationEventTypeId;";
    }
}