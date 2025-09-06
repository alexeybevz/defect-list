using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface IDeleteRootItemCommand
    {
        Task Execute(int id);
    }
}