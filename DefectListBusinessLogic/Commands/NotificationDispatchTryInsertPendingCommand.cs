using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using DefectListDomain.Commands;
using DefectListDomain.Models;
using ReporterBusinessLogic.Services.DbConnectionsFactory;

namespace DefectListBusinessLogic.Commands
{
    public class NotificationDispatchTryInsertPendingCommand : DbConnectionPmControlRepositoryBase, INotificationDispatchTryInsertPendingCommand
    {
        public NotificationDispatchTryInsertPendingCommand(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<bool> Execute(int userId, NotificationEventType notificationEventType, string payloadHash)
        {
            using (var db = await CreateOpenConnectionAsync())
            {
                try
                {
                    await db.ExecuteAsync(Query, new NotificationDispatch()
                    {
                        UserId = userId,
                        NotificationEventTypeId = notificationEventType,
                        PayloadHash = payloadHash,
                        Status = "Pending",
                        ScheduleAt = DateTime.Now,
                    });

                    return true;
                }
                catch (SqlException ex) when(ex.Number == 2601 || ex.Number == 2627)
                {
                    return false;
                }
            }
        }

        private const string Query = @"
            INSERT INTO NotificationDispatches (
                  UserId
                , NotificationEventTypeId
                , PayloadHash
                , Status
                , ScheduleAt
            ) VALUES (
                  @UserId
                , @NotificationEventTypeId
                , @PayloadHash
                , @Status
                , @ScheduleAt
            )";
    }
}