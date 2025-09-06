using System.Collections.Generic;
using System.Threading.Tasks;
using DefectListDomain.Models;

namespace DefectListDomain.Commands
{
    public interface IExpandBomItemNodeCommand
    {
        Task Execute(int assemblyBomItemId, string assemblyBomItemStructureNumber, List<BomItem> assemblyBom, bool isExpandAll = false);
    }
}