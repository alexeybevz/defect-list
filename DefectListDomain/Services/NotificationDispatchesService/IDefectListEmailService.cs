using System.Collections.Generic;
using System.Threading.Tasks;

namespace DefectListDomain.Services.NotificationDispatchesService
{
    public interface IDefectListEmailService
    {
        Task SendNotificationAsync(string recipients, string subject, string body, bool isBodyHtml, IEnumerable<string> attachments);
    }
}