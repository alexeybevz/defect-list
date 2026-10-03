using System;
using System.Threading.Tasks;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using DefectListDomain.Queries;
using DefectListDomain.Services.NotificationDispatchesService;

namespace DefectListBusinessLogic.Services.NotificationDispatchesService
{
    public class NotificationDispatchesService : INotificationDispatchesService
    {
        private readonly INotificationHandlerResolver _notificationHandlerResolver;
        private readonly INotificationDispatchTryInsertPendingCommand _tryInsertPendingNotificationDispatchCommand;
        private readonly INotificationDispatchMarkSentCommand _notificationDispatchMarkSentCommand;
        private readonly INotificationDispatchMarkFailedCommand _notificationDispatchMarkFailedCommand;
        private readonly IGetAllNotificationDispatchesQuery _getAllNotificationDispatchesQuery;

        public NotificationDispatchesService(
            INotificationHandlerResolver notificationHandlerResolver,
            INotificationDispatchTryInsertPendingCommand tryInsertPendingNotificationDispatchCommand,
            INotificationDispatchMarkSentCommand notificationDispatchMarkSentCommand,
            INotificationDispatchMarkFailedCommand notificationDispatchMarkFailedCommand,
            IGetAllNotificationDispatchesQuery getAllNotificationDispatchesQuery)
        {
            _notificationHandlerResolver = notificationHandlerResolver;
            _tryInsertPendingNotificationDispatchCommand = tryInsertPendingNotificationDispatchCommand;
            _notificationDispatchMarkSentCommand = notificationDispatchMarkSentCommand;
            _notificationDispatchMarkFailedCommand = notificationDispatchMarkFailedCommand;
            _getAllNotificationDispatchesQuery = getAllNotificationDispatchesQuery;
        }

        public async Task DailyDigestExecuteAsync(int userId, NotificationEventType notificationEventType, DateTime digestDate)
        {
            var hash = HashBuilder.BuildHash(userId, notificationEventType, digestDate);

            var created = await _tryInsertPendingNotificationDispatchCommand.Execute(userId, notificationEventType, hash);

            if (!created)
            {
                var notificationDispatch = await _getAllNotificationDispatchesQuery.Execute(hash);
                if (notificationDispatch == null || notificationDispatch.Status != "Failed")
                    return; // уже отправляли
            }

            try
            {
                var handler = _notificationHandlerResolver.Resolver(notificationEventType);
                await handler.SendAsync(userId, digestDate);

                await _notificationDispatchMarkSentCommand.Execute(hash);
            }
            catch (Exception e)
            {
                await _notificationDispatchMarkFailedCommand.Execute(hash, e.Message);
                throw; // чтобы Hangfire сделал retry
            }
        }
    }
}