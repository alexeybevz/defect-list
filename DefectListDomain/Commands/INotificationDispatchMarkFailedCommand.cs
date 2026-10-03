using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface INotificationDispatchMarkFailedCommand
    {
        Task Execute(string hash, string error);
    }
}