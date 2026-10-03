using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface INotificationDispatchTryInsertPendingCommand
    {
        Task<bool> Execute(int userId, NotificationEventType notificationEventType, string payloadHash);
    }
}