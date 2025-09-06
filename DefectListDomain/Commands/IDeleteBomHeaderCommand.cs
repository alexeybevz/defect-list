using System.Threading.Tasks;

namespace DefectListDomain.Commands
{
    public interface IDeleteBomHeaderCommand
    {
        Task Execute(int id);
    }
}