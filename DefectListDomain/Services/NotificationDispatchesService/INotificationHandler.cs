using System;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Services.NotificationDispatchesService
{
    public interface INotificationHandler
    {
        NotificationEventType EventType { get; }
        Task SendAsync(int userId, DateTime digestDate);
    }
}