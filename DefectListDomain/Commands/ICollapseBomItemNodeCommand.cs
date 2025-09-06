using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface ICollapseBomItemNodeCommand
    {
        Task Execute(int assemblyBomItemId, List<BomItem> assemblyBom);
    }
}