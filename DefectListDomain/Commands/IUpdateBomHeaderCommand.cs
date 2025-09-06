using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IUpdateBomHeaderCommand
    {
        Task Execute(BomHeader bomHeader);
    }
}