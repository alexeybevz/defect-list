using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllNotificationEventSubscribersQuery
    {
        Task<IEnumerable<NotificationEventSubscriber>> Execute();
        Task<NotificationEventSubscriber> Execute(int userId, NotificationEventType notificationEventType);
    }
}