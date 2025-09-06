using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateRootItemCommand
    {
        Task Execute(RootItem rootItem);
    }
}