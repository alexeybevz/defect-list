using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IUpdateStateBomHeaderCommand
    {
        Task Execute(BomHeader bomHeader);
    }
}