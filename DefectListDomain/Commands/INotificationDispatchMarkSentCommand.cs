using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface INotificationDispatchMarkSentCommand
    {
        Task Execute(string hash);
    }
}