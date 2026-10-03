using System;
using System.Threading.Tasks;

namespace DefectListDomain.Services.NotificationDispatchesService
{
    public interface IUserBomHeaderSubscriptionDigestBuilder
    {
        Task<string> ExecuteAsync(int userId, DateTime digestDate);
    }
}