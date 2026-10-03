using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Commands
{
    public class SubscribeUserOnBomHeaderCommand : DbConnectionPmControlRepositoryBase, ISubscribeUserOnBomHeaderCommand
    {
        private readonly IGetAllNotificationEventSubscribersQuery _getAllNotificationEventSubscribersQuery;
        private readonly ICreateNotificationEventSubscriberCommand _createNotificationEventSubscriberCommand;

        public SubscribeUserOnBomHeaderCommand(
            IDbConnectionFactory dbConnectionFactory,
            IGetAllNotificationEventSubscribersQuery getAllNotificationEventSubscribersQuery,
            ICreateNotificationEventSubscriberCommand createNotificationEventSubscriberCommand) : base(dbConnectionFactory)
        {
            _getAllNotificationEventSubscribersQuery = getAllNotificationEventSubscribersQuery;
            _createNotificationEventSubscriberCommand = createNotificationEventSubscriberCommand;
        }

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

            var isExistsEventSubscriber = await _getAllNotificationEventSubscribersQuery.Execute(
                bomHeaderSubscriber.UserId, NotificationEventType.UserBomHeaderSubscriptionDailyDigest) != null;

            if (!isExistsEventSubscriber)
            {
                await _createNotificationEventSubscriberCommand.Execute(
                    bomHeaderSubscriber.UserId, NotificationEventType.UserBomHeaderSubscriptionDailyDigest);
            }
        }
    }
}