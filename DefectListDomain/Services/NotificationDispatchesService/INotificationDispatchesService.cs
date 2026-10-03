using System;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Services.NotificationDispatchesService
{
    public interface INotificationDispatchesService
    {
        Task DailyDigestExecuteAsync(int userId, NotificationEventType notificationEventType, DateTime digestDate);
    }
}