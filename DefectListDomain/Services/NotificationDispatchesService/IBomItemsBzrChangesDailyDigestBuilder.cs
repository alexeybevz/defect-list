using System;
using System.Threading.Tasks;

namespace DefectListDomain.Services.NotificationDispatchesService
{
    public interface IBomItemsBzrChangesDailyDigestBuilder
    {
        Task<string> ExecuteAsync(DateTime digestDate);
    }
}