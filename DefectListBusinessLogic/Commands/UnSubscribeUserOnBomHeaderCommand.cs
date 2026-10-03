using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ReporterBusinessLogic.Services.DbConnectionsFactory;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;

namespace DefectListBusinessLogic.Commands
{
    public class UnSubscribeUserOnBomHeaderCommand : DbConnectionPmControlRepositoryBase, IUnSubscribeUserOnBomHeaderCommand
    {
        private readonly IGetAllBomHeaderSubscribersQuery _getAllBomHeaderSubscribersQuery;
        private readonly IDeleteNotificationEventSubscriberCommand _deleteNotificationEventSubscriberCommand;

        public UnSubscribeUserOnBomHeaderCommand(
            IDbConnectionFactory dbConnectionFactory,
            IGetAllBomHeaderSubscribersQuery getAllBomHeaderSubscribersQuery,
            IDeleteNotificationEventSubscriberCommand deleteNotificationEventSubscriberCommand) : base(dbConnectionFactory)
        {
            _getAllBomHeaderSubscribersQuery = getAllBomHeaderSubscribersQuery;
            _deleteNotificationEventSubscriberCommand = deleteNotificationEventSubscriberCommand;
        }

        public async Task Execute(BomHeaderSubscriber bomHeaderSubscriber)
        {
            var query = "DELETE FROM BomHeaderSubscribers WHERE UserId = @UserId AND BomId = @BomId;";
            using (var db = await CreateOpenConnectionAsync())
            {
                await db.ExecuteAsync(query, new { bomHeaderSubscriber.UserId, bomHeaderSubscriber.BomId });
            }

            var bomHeaders = await _getAllBomHeaderSubscribersQuery.Execute(bomHeaderSubscriber.UserId);
            if (!bomHeaders.Any())
            {
                await _deleteNotificationEventSubscriberCommand.Execute(bomHeaderSubscriber.UserId,
                    NotificationEventType.UserBomHeaderSubscriptionDailyDigest);
            }
        }
    }
}