using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ILoadBomItemsToBomHeaderCommand
    {
        Task Execute(BomHeader bomHeader, string userLogin);
    }
}