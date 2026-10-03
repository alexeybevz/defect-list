using System.Collections.Generic;
using System.Linq;
using DefectListDomain.Models;
using DefectListDomain.Services.NotificationDispatchesService;

namespace DefectListBusinessLogic.Services.NotificationDispatchesService
{
    public class NotificationHandlerResolver : INotificationHandlerResolver
    {
        private readonly IEnumerable<INotificationHandler> _handlers;

        public NotificationHandlerResolver(IEnumerable<INotificationHandler> handlers)
        {
            _handlers = handlers;
        }

        public INotificationHandler Resolver(NotificationEventType type)
        {
            return _handlers.First(x => x.EventType == type);
        }
    }
}