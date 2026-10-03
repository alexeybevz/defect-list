using DefectListDomain.Models;

namespace DefectListDomain.Services.NotificationDispatchesService
{
    public interface INotificationHandlerResolver
    {
        INotificationHandler Resolver(NotificationEventType type);
    }
}