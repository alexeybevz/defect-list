using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IUpdateBomItemQtyCommand
    {
        Task Execute(int bomItemId, BomItem bomItem, string userLogin);
    }
}