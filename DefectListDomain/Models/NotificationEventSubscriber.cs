using System;

namespace DefectListDomain.Models
{
    public class NotificationEventSubscriber
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public NotificationEventType NotificationEventType { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}