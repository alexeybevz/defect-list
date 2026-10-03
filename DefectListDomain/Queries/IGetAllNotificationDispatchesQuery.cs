using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Queries
{
    public interface IGetAllNotificationDispatchesQuery
    {
        Task<NotificationDispatch> Execute(string hash);
    }
}