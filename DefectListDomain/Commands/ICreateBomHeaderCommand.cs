using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICreateBomHeaderCommand
    {
        Task Execute(BomHeader bomHeader);
    }
}