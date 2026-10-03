using System;

namespace DefectListDomain.Models
{
    public class NotificationDispatch
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public NotificationEventType NotificationEventTypeId { get; set; }
        public string PayloadHash { get; set; }
        public string Status { get; set; }
        public int Attempts { get; set; }
        public DateTime ScheduleAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? LastAttempAt { get; set; }
        public string Error { get; set; }
    }
}