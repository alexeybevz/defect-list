using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateNotificationEventSubscriberCommand
    {
        Task Execute(int userId, NotificationEventType notificationEventType);
    }
}