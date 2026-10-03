using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IDeleteNotificationEventSubscriberCommand
    {
        Task Execute(int userId, NotificationEventType notificationEventType);
    }
}